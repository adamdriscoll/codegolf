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

        $.post(this.props.dataUrl,
        {
            item: this.props.itemId,
            value: this.props.upvote ? 1 : -1
        }, this.handleSuccessfulVote.bind(this));
    }

    handleSuccessfulVote() {
        if (this.props.onVoted) {
            this.props.onVoted();
        }

        this.state.voted = true;
        this.setState(this.state);
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
                    <a onclick={this.handleOnClick.bind(this)}>
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
            upvoted: true,
            downvoted: true
        }

        if (props.votes) {
            this.state.votes = props.votes;
        }
    }

    handleUpVote() {
        handleVote(1);
    }

    handleDownVote() {
        handleVote(-1);
    }

    handleVote(voteValue) {
        var self = this;
        $.post(this.props.dataUrl,
            () => {
                self.state.votes += voteValue;
                self.state.upvoted = true;
                self.state.downvoted = false;
                self.setState(self.state);
            });
    }

    render() {
        const canUpvote = !this.state.upvoted;
        const canDownvote = !this.state.downvoted;

        return (<div className="row">
                    <div className="col-md-3">
                        <VoteButton upvote={true} onVoted={this.handleUpVote.bind(this)} canVote={canUpvote}/>
                    </div>
                    <div className="col-md-3">
                        {this.state.votes}
                    </div>
                    <div className="col-md-3">
                        <VoteButton upvote={false} onVoted={this.handleDownVote.bind(this)} canVote={canDownvote}/>
                    </div>
                </div>);
    }
}