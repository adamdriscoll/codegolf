class SolutionRow extends React.Component {
    render() {
        var deleteLink = null;

        if (this.props.author.isCurrentUser) {
            deleteLink = <a href={this.props.deleteSolutionUrl}><i className="fa fa-trash-o fa-lg"></i></a>;
        }

        var authIcon = null;
        if (this.props.authorAuthType === "github") {
            authIcon = <i className="fa fa-github fa-lg"></i>;
        }

        return(
                    <tr>
                            <td>
                                <VoteButtons upvoteUrl={this.props.upvoteUrl} downvoteUrl={this.props.downvoteUrl} votes={this.props.votes} />
                            </td>
                            <td><a href={this.props.author.profileUrl}>{authIcon} {this.props.author.name}</a></td>
                            <td><span className="badge">{this.props.solutionLength}</span></td>
                            <td>{this.props.solutionDate}</td>
                            <td>{deleteLink}</td>
                            <td></td>
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
        return <SolutionRow  
                             deleteSolutionUrl={solution.deleteSolutionUrl}
                             author={solution.author}
                             solutionLength={solution.length}
                             solutionDate={solution.date}
                             downvoteUrl={solution.downvoteUrl}
                             upvoteUrl={solution.upvoteUrl}
                             votes={solution.votes}/>;
    }

    componentWillMount() {
        var self = this;
        $.get(this.props.solutionDataUrl,
            function(data) {
                self.state.solutions = data;
                self.setState(self.state);
            });
    }

    render() {
        const solutionRows = this.state.solutions.map(solution => this.renderSolution(solution));

        return (<table className="table">
                    <tr>
                        <th>Crowd Appeal</th>
                        <th>Player</th>
                        <th>Score</th>
                        <th>Date Played</th>
                        <th></th>
                        <th></th>
                    </tr>
                    {solutionRows}
                </table>);
    }
}