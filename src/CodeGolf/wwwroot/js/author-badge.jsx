class AuthorBadge extends React.Component {
    render() {
        var authIcon = null;
        if (this.props.authType === "github") {
            authIcon = <i className="fa fa-github fa-lg"></i>;
        }

        return <a href={this.props.profileUrl}>{authIcon} {this.props.name}</a>;
    }
}