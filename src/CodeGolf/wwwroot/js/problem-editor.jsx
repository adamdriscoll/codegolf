class ProblemEditor extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            title: "",
            description: "",
            testCases: [{ input: "", output: "" }],
            language: "",
            selectedLanguage: null,
            availableLanguages: [],
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

    onLanguageChange(event) {
        this.state.language = event.target.value;
        this.state.canSubmit = this.checkCanSubmit();

        this.state.availableLanguages.forEach(lang => {
            if (lang.id === event.target.value) {
                this.state.showEnforceCheckBox = lang.supportsValidation;
                this.state.selectedLanguage = lang;
            }
        });

        this.setState(this.state);
    }

    onEnforceOutputChanged(event) {
        this.state.enforceOutput = event.target.value;
        this.state.canSubmit = this.checkCanSubmit();
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

        return titleSpecified && descriptionSpecified && testCaseOutputPopulated;
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
                self.setState(self.state);
            });
        }

        $.get(this.props.languagesUrl,
            function(data) {
                self.state.availableLanguages = data;
                self.setState(self.state);
            });
    }

    renderLanguageOptions(language) {
        return <option value={language.id} key={language.id}>{language.displayName}</option>;
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
        $.post(this.state.postUrl, {
            name: this.state.title,
            description: this.state.description,
            testCases: this.state.testCases,
            language: this.state.language,
            enforceOutput: this.state.enforceOutput
        },
            function(data) {
                window.location.href = data;
            });
    }

    render() {
        const languageOptions = this.state.availableLanguages.map((lang) => this.renderLanguageOptions(lang));

        let testCaseIndex = 0;
        const testCases = this.state.testCases.map((testCase) => this.renderTestCaseEditor(testCaseIndex++, testCase));

        let submit = <input className="btn btn-default" type="button" value="Submit" disabled/>;
        if (this.state.canSubmit) {
            submit = <input className="btn btn-default" type="button" value="Submit" onClick={this.submit.bind(this)}/>;
        }

        let enforceCheckBox = null;
        if (this.state.showEnforceCheckBox) {
            enforceCheckBox = <div className="form-group">
                                  <input type="checkbox" id="EnforceOutput" value={this.state.enforceOutput}/>
                                  <label htmlFor="EnforceOutput">Enforce Output</label>
                                  <br/>
                                  <small>Require solutions to match output before they can be submitted.</small>
                              </div>;
        }

        return (
            <div>
                <div className="form-group">
                    <label htmlFor="name">Name</label>
                    <input type="text" className="form-control" placeholder="Name" value={this.state.title} onChange={this.onNameChanged.bind(this)} />
                </div>
                <div className="form-group">
                    <label htmlFor="name">Language</label>
                    <select className="form-control" value={this.state.language} onChange={this.onLanguageChange.bind(this)}>
                        {languageOptions}
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="description">Description</label>
                    <br />
                    <small>Describe your problem. You can use markdown in the editor below.</small>
                    <MonacoEditor contents={this.state.description} onContentChanged={this.onDescriptionContentChanged.bind(this)} waitForContent={true}/>
                </div>
                {testCases}

                <input className="btn btn-default" type="button" value="Add Test Case" onClick={this.addTestCase.bind(this)} />

                {enforceCheckBox}
                {submit}
            </div>
        );
    }
}