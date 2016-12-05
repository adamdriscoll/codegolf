class ProblemEditor extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            title: "",
            description: "",
            testCases: [{ input: "", output: "" }],
            anyLanguage: true,
            language: "",
            enforceOutput: false,
            canSubmit: false,
            postUrl: props.postUrl,
            showEnforceCheckBox: false
        }
    }

    onNameChanged(event) {
        this.state.title = event.target.value;
        this.state.canSubmit = this.checkCanSubmit();
        this.setState(this.state);
    }
    
    onDescriptionContentChanged(content) {
        this.state.description = content;
        this.state.canSubmit = this.checkCanSubmit();
        this.setState(this.state);
    }

    handleOnSelectedLanguageChanged(lang, langName) {
        this.state.language = langName;
        this.state.canSubmit = this.checkCanSubmit();
        this.state.showEnforceCheckBox = lang.canValidate;
        this.setState(this.state);
    }

    onEnforceOutputChanged(event) {
        this.state.enforceOutput = event.target.checked;
        this.state.canSubmit = this.checkCanSubmit();
        this.setState(this.state);
    }

    onAnyLanguageChange(event) {
        this.state.enforceOutput = false;
        this.state.showEnforceCheckBox = false;
        this.state.anyLanguage = event.target.checked;
        this.setState(this.state);
    }

    checkCanSubmit() {
        const titleSpecified = this.state.title != null && this.state.title !== "";
        const descriptionSpecified = this.state.description != null && this.state.description !== "";

        const testCaseSpecified = this.state.testCases.length > 0;

        var testCaseOutputPopulated = true;
        if (testCaseSpecified) {
            this.state.testCases.forEach((testCase) => {
                testCaseOutputPopulated = testCaseOutputPopulated && testCase.output != null && testCase.output !== "";
            });
        }

        var languageCorrectLength = this.state.language.length < 15;

        return titleSpecified && descriptionSpecified && testCaseOutputPopulated && languageCorrectLength;
    }

    componentWillMount() {
        const self = this;

        if (this.props.problemUrl) {
            $.get(this.props.problemUrl,
            function (data) {
                self.state.title = data.name;
                self.state.description = data.description;
                self.state.testCases = data.testCases;
                self.state.language = data.language;
                self.state.anyLanguage = data.anyLanguage;
                self.setState(self.state);
            });
        }
    }

    onTestCaseChanged(testCaseIndex, input, output) {
        this.state.testCases[testCaseIndex].input = input;
        this.state.testCases[testCaseIndex].output = output;
        this.state.canSubmit = this.checkCanSubmit();
        this.setState(this.state);
    }

    removeTestCase(e) {
        const testCaseIndex = e.currentTarget.dataset.testcase;

        this.state.testCases.splice(testCaseIndex, 1);
        this.setState(this.state);
    }

    addTestCase() {
        this.state.testCases.push({ input: "", output: "" });
        this.setState(this.state);
    }

    renderTestCaseEditor(testCaseIndex, testCase) {
        return <div className="panel panel-default">
                   <div className="panel-heading">Test Case <a href="#!" onClick={this.removeTestCase.bind(this)} data-testcase={testCaseIndex}><i className="fa fa-times fa-lg" ></i></a></div>
                   <div className="panel-body">
                       <TestCaseEditor index={testCaseIndex} input={testCase.input} output={testCase.output} language={this.state.selectedLanguage} onTestCaseChanged={this.onTestCaseChanged.bind(this)} />
                   </div>
               </div>;
    }

    submit() {
        if (this.state.anyLanguage) {
            this.state.language = "Any";
        }

        $.post(this.state.postUrl, {
            name: this.state.title,
            description: this.state.description,
            testCases: this.state.testCases,
            language: this.state.language,
            enforceOutput: this.state.enforceOutput,
            anyLanguage: this.state.anyLanguage
        },
            function(data) {
                window.location.href = data;
            });
    }

    render() {
        let testCaseIndex = 0;
        const testCases = this.state.testCases.map((testCase) => this.renderTestCaseEditor(testCaseIndex++, testCase));

        let submit = <input className="btn btn-default" type="button" value="Submit" disabled/>;
        if (this.state.canSubmit) {
            submit = <input className="btn btn-default" type="button" value="Submit" onClick={this.submit.bind(this)}/>;
        }

        let enforceCheckBox = null;
        if (this.state.showEnforceCheckBox) {
            enforceCheckBox = <div className="form-group">
                                  <input type="checkbox" id="EnforceOutput" checked={this.state.enforceOutput} onChange={this.onEnforceOutputChanged.bind(this)}/>
                                  <label htmlFor="EnforceOutput">Enforce Output</label>
                                  <br/>
                                  <small>Require solutions to match output before they can be submitted.</small>
                              </div>;
        }

        let languageSelector = null;
        if (!this.state.anyLanguage) {
            languageSelector = <LanguageSelector languagesUrl={this.props.languagesUrl} onSelectedLanguageChanged={this.handleOnSelectedLanguageChanged.bind(this)} language={this.state.language}/>
        }

        return (
            <div>
                <div className="form-group">
                    <label htmlFor="name">Name</label>
                    <input type="text" className="form-control" placeholder="Name" value={this.state.title} onChange={this.onNameChanged.bind(this)} />
                </div>
                <div className="form-group">
                                  <input type="checkbox" id="anyLanguage" checked={this.state.anyLanguage} onChange={this.onAnyLanguageChange.bind(this)}/>
                                  <label htmlFor="anyLanguage">Any Language</label>
                </div>
                {languageSelector}
                {enforceCheckBox}
                <div className="form-group">
                    <label htmlFor="description">Description</label>
                    <br />
                    <small>Describe your problem. You can use markdown in the editor below.</small>
                    <MonacoEditor contents={this.state.description} onContentChanged={this.onDescriptionContentChanged.bind(this)} waitForContent={true}/>
                </div>
                {testCases}

                <input className="btn btn-default" type="button" value="Add Test Case" onClick={this.addTestCase.bind(this)} />

                {submit}
            </div>
        );
    }
}