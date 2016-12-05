class SolutionValidator extends React.Component {
    constructor() {
        super();

        this.state = {
            validationState: "unknown",
            solutionOutput: ""
        }
    }

    handleOnValidationStateChanged() {
        if (this.props.onValidationStateChanged) {
            this.props.onValidationStateChanged(this.state.validationState, this.state.solutionOutput);
        }
    }

    testSolution() {
        if (this.state.validationState === "running") {
            return;
        }

        this.state.validationState = "running";
        this.setState(this.state);
        var self = this;

        $.post(this.props.solutionValidationUrl, {
                problem: this.props.problemId,
                content: this.props.solutionContent
            },
            function(data) {
                self.state.validationState = data.passed ? "passed" : "failed";
                self.state.solutionOutput = data.testCaseResults[0].actualOutput;
                self.setState(self.state);

                self.handleOnValidationStateChanged();
            }).fail(function () {
                self.state.validationState = "failed";
                self.state.solutionOutput = "Error during validation!";
                self.setState(self.state);

                self.handleOnValidationStateChanged();
            });
    }

    render() {
        let statusBadge = <span><i className="fa fa-question fa-lg"></i> Unknown solution status.</span>;
        switch(this.state.validationState) {
        case "running":
            statusBadge = <span><i className="fa fa-circle-o-notch fa-spin"></i> Testing solution...</span>;
            break;
        case "passed":
            statusBadge = <span><i className="fa fa-check fa-lg"></i> Solution Successful!</span>;
            break;
        case "failed":
            statusBadge = <span><i className="fa fa-times fa-lg"></i> Solution Failed. See output below.</span>;
            break;
        }

        return (
            <div>
                <div className="col-md-2">
                    <a href="#!" onClick={this.testSolution.bind(this)}><i className="fa fa-play fa-lg"></i> Run</a>
                </div>
                <div className="col-md-4">
                    {statusBadge}
                </div>
            </div>);
    }
}

class SolutionEditor extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            solutionContent: "",
            solutionOutput: "",
            language: props.language,
            canSubmitSolution: !props.enforceOutput,
            solutionLength: 0
        }
    }

    submitSolution() {
        this.state.canSubmitSolution = false;
        this.setState(this.state);

        $.post(this.props.newSolutionUrl,
            { content: this.state.solutionContent, problemId: this.props.problemId, language: this.state.language },
            function() {
                window.location.reload();
            });
    }

    handleOnSelectedLanguageChanged(lang, langName) {
        this.state.language = langName;
        this.setState(this.state);
    }

    solutionContentChanged(content) {
        this.state.solutionContent = content;
        this.state.solutionLength = content.length;
        this.setState(this.state);
    }

    onValidationStateChanged(state, output) {
        this.state.solutionOutput = output;

        if (this.props.enforceOutput) {
            this.state.canSubmitSolution = state === "passed";
        } else {
            this.state.canSubmitSolution = true;
        }

        this.setState(this.state);
    }

    renderSolutionValidator() {
        if (!this.props.supportsValidation) {
            return null;
        }
        return <SolutionValidator solutionContent={this.state.solutionContent}
                                  solutionValidationUrl={this.props.solutionValidationUrl} 
                                  problemId={this.props.problemId}
                                  onValidationStateChanged={this.onValidationStateChanged.bind(this)}
                                  />;
    }

    renderEnforceOutput() {
        if (!this.props.enforceOutput) {
            return null;
        }

        return <small>This problem requires that you match the output exactly.</small>;
    }

    renderSolutionOutput() {
        if (!this.props.supportsValidation || this.state.solutionOutput == null || this.state.solutionOutput.length === 0) {
            return null;
        }

        return <div><h3>Output</h3><pre>{this.state.solutionOutput}</pre></div>;
    }

    render() {
        const solutionValidator = this.renderSolutionValidator();
        const enforceOutput = this.renderEnforceOutput();
        const solutionOutput = this.renderSolutionOutput();
        
        let submitButton = <input className="btn btn-default" type="button" value="Submit" onClick={this.submitSolution.bind(this)}/>;
        if (!this.state.canSubmitSolution) {
            submitButton = <input className="btn  btn-default" type="button" value="Submit" disabled/>;
        }

        let languageSelector = null;
        if (this.props.canSelectLanguage) {
            languageSelector = <LanguageSelector onSelectedLanguageChanged={this.handleOnSelectedLanguageChanged.bind(this)}/>;
        }

        return (
            <div>
                <h3>Play a round</h3>
                <hr />
                <div style={{paddingBottom: 10}} className="row">
                    <div className="col-md-2">
                        Strokes <span className="badge">{this.state.solutionLength}</span>
                    </div>
                    {solutionValidator}
                </div>
                {languageSelector}
                {enforceOutput}
                <div className="row">
                    <div className="col-md-12">
                        <MonacoEditor onContentChanged={this.solutionContentChanged.bind(this)} />
                    </div>
                </div>
                <div>
                    {solutionOutput}
                </div>
                <div>
                    {submitButton}
                </div>
            </div>
            );
    }
}