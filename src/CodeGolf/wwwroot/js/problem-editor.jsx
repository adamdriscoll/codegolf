class ProblemEditor extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            title: "",
            description: "",
            testCases: [{ input: "", output: "" }],
            language: "",
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
    
    onInputContentChanged(content) {
        this.state.testCases[0].input = content;
        this.state.canSubmit = this.checkCanSubmit();
        this.setState(this.state);
    }

    onOutputContentChanged(content) {
        this.state.testCases[0].output = content;
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
        const outputSpecified = this.state.testCases[0].output != null && this.state.testCases[0].output !== "";

        return titleSpecified && descriptionSpecified && outputSpecified;
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
                <div className="form-group">
                    <label htmlFor="input">Input</label>
                    <br />
                    <small>Define the inputs to your problem. Input isn't required. Input should be valid syntax for the language you select.</small>
                    <MonacoEditor contents={this.state.testCases[0].input} onContentChanged={this.onInputContentChanged.bind(this)} waitForContent={true}/>
                </div>
                <div className="form-group">
                    <label htmlFor="output">Expected Output</label>
                    <br />
                    <small>Define outputs to your problem.</small>
                    <MonacoEditor contents={this.state.testCases[0].output} onContentChanged={this.onOutputContentChanged.bind(this)} waitForContent={true}/>
                </div>
                {enforceCheckBox}
                {submit}
            </div>
        );
    }
}