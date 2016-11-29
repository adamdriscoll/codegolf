class DrivingRange extends React.Component {
    constructor() {
        super();

        this.state = {
            languages: [],
            content: "",
            output: "",
            selectedLanguage: "",
            executing: false
        }
    }

    onLanguageChange(event) {
        this.state.selectedLanguage = event.target.value;
        this.setState(this.state);
    }

    renderLanguageOptions(language) {
        return <option value={language} key={language}>{language}</option>;
    }

    writeOutput(str) {
        this.state.output += str + "\n";
        this.setState(this.state);
    }

    clearOutput() {
        this.state.output = "";
        this.setState(this.state);
    }

    onExecute() {
        if (this.state.executing) {
            return;
        }

        this.state.executing = true;
        this.setState(this.state);

        this.writeOutput("Executing....");

        const self = this;

        $.post(this.props.executeUrl,
            {
                content: self.state.content,
                language: self.state.selectedLanguage
            },
            function(result) {
                self.writeOutput(result);
                self.state.executing = false;
                self.setState(self.state);
            }).fail(function () {
                self.state.executing = false;
                self.setState(self.state);
                self.writeOutput("Error during validation!");
            });
    }

    onContentChanged(content) {
        this.state.content = content;
        this.setState(this.state);
    }

    componentWillMount() {
        const self = this;

        $.get(this.props.languageUrl,
            function(data) {
                self.state.languages = data;
                self.state.selectedLanguage = data[0];
                self.setState(self.state);
            });
    }

    render() {
        const languageOptions = this.state.languages.map((lang) => this.renderLanguageOptions(lang));

        let submitButton = <input className="btn btn-default" type="button" value="Execute" onClick={this.onExecute.bind(this) }/>;
        if (this.state.executing) {
            submitButton = <input className="btn  btn-default" type="button" value="Execute" disabled/>;
        }

        return <div>
                    <div className="row">
                        <div className="col-md-3">
                                                    <div className="form-group">
                        <select className="form-control" value={this.state.selectedLanguage} onChange={this.onLanguageChange.bind(this)}>{languageOptions}
                        </select>
                                                    </div>

                    </div>
                        <div className="col-md-1">
                            {submitButton}
                        </div>
                        <div className="col-md-1">
                            <input className="btn btn-default" type="button" value="Clear Output" onClick={this.clearOutput.bind(this) } />
                        </div>
                    </div>

                   
                    <MonacoEditor onContentChanged={this.onContentChanged.bind(this)} />
                    
                    <div><pre>{this.state.output}</pre></div>
                </div>;
    }
}