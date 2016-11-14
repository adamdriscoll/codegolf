class ProblemDescription extends React.Component {
    render() {
        return (
            <div>
                <p>
                    {this.props.description}
                </p>

                <h3>Input</h3>
                <pre>{this.props.input}</pre>

                <h3>Expected Output</h3>
                <pre>{this.props.output}</pre>
            </div>
        );
    }
}

class Problem extends React.Component {
    render() {
        let editLink = null;
        if (this.props.isAuthor) {
            editLink = <div className="col-md-1">
                            <a href={this.props.editProblemUrl}>
                                <i className="fa fa-pencil-square-o fa-lg"></i> Edit
                            </a>
            </div>;
        }

        let solutionEditor;
        if (this.props.canAddSolution) {
            solutionEditor = <SolutionEditor
                                problemId={this.props.problemId}
                                newSolutionUrl={this.props.newSolutionUrl}
                                problemUrl={this.props.problemUrl}
                                supportsValidation={this.props.languageSupportsValidation}
                                solutionValidationUrl={this.props.solutionValidationUrl}
                                enforceOutput={this.props.enforceSolutionOutput}/>;
        } else {
            solutionEditor = <h2>Login to play!</h2>;
        }

        return (
        <div>
         <div className="row">
            <div className="col-md-12">
                <h2>{this.props.name}</h2>
            </div>
         </div>
            <div className="row">
                <div className="col-md-2">
                    by <AuthorBadge profileUrl={this.props.authorProfileUrl} authType={this.props.authorAuthType} name={this.props.authorName} />
                </div>
                <div className="col-md-1">
                    <span className="label label-success">{this.props.language}</span>
                </div>
                {editLink}
            </div>
            <hr/>
            <ProblemDescription description={this.props.problemDescription} input={this.props.problemInput} output={this.props.problemOutput} />
            <h3>Rounds</h3>
            <SolutionTable solutionDataUrl={this.props.solutionDataUrl} />
            <div className="row">
                <div className="col-md-12">
                    {solutionEditor}
                </div>
            </div>
        </div>

       );
    }
}