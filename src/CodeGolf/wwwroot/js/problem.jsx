class ProblemDescription extends React.Component {
    constructor() {
        super();

        this.state = {
            descriptionHtml: ""
        }
    }

    componentWillMount() {
        var converter = new showdown.Converter();
        var text = this.props.description;
        var rawHtml = converter.makeHtml(text);
        this.state.descriptionHtml = <div dangerouslySetInnerHTML={{ __html: rawHtml } }></div>;
        this.setState(this.state);
    }

    renderTestCase(testCase) {
        return <div className="panel panel-default">
                    <div className="panel-body">
                       <b>Input</b>
                       <pre>{testCase.input}</pre>
                       <b>Expected Output</b>
                       <pre>{testCase.output}</pre>
                    </div>
               </div>;
    }

    render() {
        const testCases = this.props.testCases.map((testCase) => this.renderTestCase(testCase));

        return (
            <div>
                <p>
                    {this.state.descriptionHtml}
                </p>
                <h3>Test Cases</h3>
                {testCases}
               
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
                                canSelectLanguage={this.props.anyLanguage}
                                problemId={this.props.problemId}
                                newSolutionUrl={this.props.newSolutionUrl}
                                problemUrl={this.props.problemUrl}
                                language={this.props.language}
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
            <ProblemDescription description={this.props.problemDescription} testCases={this.props.testCases} />
            <h3>Rounds</h3>
            <SolutionTable solutionDataUrl={this.props.solutionDataUrl} problemLanguage={this.props.language} />
            <div className="row">
                <div className="col-md-12">
                    {solutionEditor}
                </div>
            </div>
        </div>

       );
    }
}