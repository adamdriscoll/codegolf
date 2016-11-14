class VoteButton extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            voted: props.voted,
            canVote: props.canVote
        }
    }

    handleOnClick() {
        if (this.state.canVote) {
            if (this.props.onVoted) {
                this.props.onVoted();
            }
        }
    }

    render() {
        var iconClass;
        if (this.props.upvote) {
            iconClass = "fa fa-arrow-up fa-lg";
        } else {
            iconClass = "fa fa-arrow-down fa-lg";
        }

        var style;
        if (this.state.voted) {
            style = "font-weigth: bold";
        }

        return (<span>
                    <a onClick={this.handleOnClick.bind(this)} href="#!">
                        <i className={iconClass} style={style}></i>
                    </a>
                </span>);
    }
}

class VoteButtons extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            votes: 0,
            upvoted: false,
            downvoted: false
        }

        if (props.votes) {
            this.state.votes = props.votes;
        }
    }

    handleUpVote() {
        this.handleVote(this.props.upvoteUrl, true);
    }

    handleDownVote() {
        this.handleVote(this.props.downvoteUrl, false);
    }

    handleVote(voteUrl, upvote) {
        var self = this;
        $.post(voteUrl,
            (votes) => {
                self.state.votes = votes;
                self.state.upvoted = upvote;
                self.state.downvoted = !upvote;
                self.setState(self.state);
            });
    }

    render() {
        const canUpvote = !this.state.upvoted;
        const canDownvote = !this.state.downvoted;

        return (<div className="row">
                    <div className="col-md-1">
                        <VoteButton upvote={true} onVoted={this.handleUpVote.bind(this)} canVote={canUpvote}/>
                    </div>
                    <div className="col-md-1">
                        {this.state.votes}
                    </div>
                    <div className="col-md-1">
                        <VoteButton upvote={false} onVoted={this.handleDownVote.bind(this)} canVote={canDownvote}/>
                    </div>
                </div>);
    }
}