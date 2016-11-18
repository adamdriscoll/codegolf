class SolutionCommentEditor extends React.Component {
    constructor() {
        super();

        this.state = {
            comment: ""
        }
    }

    onCommentChanged(e) {
        this.state.comment = e.target.value;
        this.setState(this.state);
    }

    onAddComment() {
        if (this.state.comment === "") {
            return;
        }

        this.props.onAddComment(this.state.comment);

        this.state.comment = "";
        this.setState(this.state);
    }

    render() {
        return (<div className="row">
                   <div className="col-md-9">
                       <input type="text" placeholder="Comment" className="input commentEditor" value={this.state.comment} onChange={this.onCommentChanged.bind(this)} />
                   </div>
                    <div className="col-md-3">
                        <input type="button" value="Add" className="btn" onClick={this.onAddComment.bind(this)} />
                    </div>
        </div>);
    }
}

class SolutionComment extends React.Component {
    constructor() {
        super();
    }

    onDeleteComment() {
        this.props.onDeleteComment(this.props.deleteUrl);
    }

    render() {
        let deleteLink = null;
        if (this.props.commentor.isCurrentUser) {
            deleteLink = <a href="#!"><i className="fa fa-trash" onClick={this.onDeleteComment.bind(this)}></i></a>;
        }

        return (<div className="comment">
            <hr/>
            {deleteLink} {this.props.comment} - <a href={this.props.commentor.profileUrl}>{this.props.commentor.name}</a> 
        </div>);
    }
}

class SolutionViewer extends React.Component {
    constructor() {
        super();

        this.state = {
            content: "",
            language: "",
            upvoteUrl: "",
            downvoteUrl: "",
            addCommentUrl: "",
            votes: 0,
            comments: []
        }
    }

    onOpen() {
        $(this.modal).modal("show");
        var self = this;
        $.get(this.props.solutionUrl,
            function (data) {
                self.state.content = data.content;
                self.state.langauge = data.language;
                self.state.downvoteUrl = data.downvoteUrl;
                self.state.upvoteUrl = data.upvoteUrl;
                self.state.addCommentUrl = data.addCommentUrl;
                self.state.votes = data.votes;
                self.state.comments = data.comments;

                self.setState(self.state);
                $('pre code').each(function (i, block) {
                    hljs.highlightBlock(block);
                });
            });
    }

    onDeleteComment(deleteUrl) {
        var self = this;

        $.ajax({
            url: deleteUrl,
            type: "DELETE",
            success: function () {
                for (var i = self.state.comments.length - 1; i >= 0; i--) {
                    if (self.state.comments[i].deleteCommentUrl === deleteUrl) {
                        self.state.comments.splice(i, 1);
                        self.setState(self.state);
                    }
                }
            }
        });
    }

    onAddComment(comment) {
        var self = this;
        $.post(this.state.addCommentUrl,
            { comment: comment },
            function (data) {
                self.state.comments.push(data);
                self.setState(self.state);
            });
    }

    render() {
        let modalContent = <i className="fa fa-spinner fa-spin fa-3x fa-fw"></i>;
        if (this.state.content !== "") {
            modalContent = <pre>
            <code>
                                {this.state.content}
                            </code>
                            </pre>;
        }

        const solutionComments = this.state.comments.map((comment) => <SolutionComment key={comment.id} commentor={comment.commentor} comment={comment.comment} deleteUrl={comment.deleteCommentUrl} onDeleteComment={this.onDeleteComment.bind(this)} />);

        return (
            <div>
                <a href="#!" onClick={this.onOpen.bind(this)}>
                    <i className="fa fa-eye fa-lg"></i>
                </a>
                <div className="modal fade" tabindex="-1" role="dialog" ref={(modal) => this.modal = modal}>
                  <div className="modal-dialog modal-lg" role="document">
                    <div className="modal-content">
                         <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 className="modal-title">Solution</h4>
                         </div>
                        <div className="modal-body">
                            <div className="row">
                                <div className="col-md-1">
                                    <VoteButtons upvoteUrl={this.props.upvoteUrl} downvoteUrl={this.props.downvoteUrl} votes={this.props.votes} />
                                </div>
                                <div className="col-md-11">
                                    {modalContent}
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-md-11 col-md-offset-1">
                                    {solutionComments}
                                </div>
                            </div>

                            <div className="row">
                                <div className="col-md-11 col-md-offset-1">
                                    <hr/>
                                    <SolutionCommentEditor onAddComment={this.onAddComment.bind(this)} />
                                </div>
                            </div>
                        </div>
                          <div className="modal-footer">
                            <button type="button" className="btn btn-default" data-dismiss="modal">Close</button>
                          </div>
                    </div>
                  </div>
                </div>
            </div>
            );
    }
}

class SolutionRow extends React.Component {
    render() {
        var deleteLink = null;

        if (this.props.author.isCurrentUser) {
            deleteLink = <a href={this.props.deleteSolutionUrl }><i className="fa fa-trash-o fa-lg"></i></a>;
        }

        var localDatetime = moment(this.props.solutionDate);

        return (
                <tr>
                        <td><SolutionViewer solutionUrl={this.props.contentUrl} /></td>
                        <td>
                            {this.props.votes}
                        </td>
                        <td><AuthorBadge profileUrl={this.props.author.profileUrl} authType={this.props.author.authType} name={this.props.author.name} /></td>
                        <td><span className="badge">{this.props.solutionLength}</span></td>
                        <td>{localDatetime.format("MMMM Do YYYY, h:mm:ss a")}</td>
                        <td>{deleteLink}</td>

                </tr>);
    }
}

class SolutionTable extends React.Component {
    constructor() {
        super();

        this.state = {
            solutions: []
        };
    }

    renderSolution(solution) {
        return <SolutionRow deleteSolutionUrl={solution.deleteSolutionUrl}
                            author={solution.author}
                            solutionLength={solution.length}
                            solutionDate={solution.date}
                            contentUrl={solution.contentUrl}
                            votes={solution.votes} />;
    }

    componentWillMount() {
        var self = this;
        $.get(this.props.solutionDataUrl,
            function (data) {
                self.state.solutions = data;
                self.setState(self.state);
            });
    }

    render() {
        const solutionRows = this.state.solutions.map(solution => this.renderSolution(solution));

        return (<table className="table">
                    <tr>
                        <th>View</th>
                        <th>Crowd Appeal</th>
                        <th>Player</th>
                        <th>Score</th>
                        <th>Date Played</th>
                        <th></th>
                    </tr>
            {solutionRows}
        </table>);
    }
}