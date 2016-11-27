class TestCaseEditor extends React.Component {
    onInputContentChanged(content) {
        this.props.input = content;
        this.handleOnTestCaseChanged();
        this.setState(this.state);
    }

    onOutputContentChanged(content) {
        this.props.output = content;
        this.handleOnTestCaseChanged();
        this.setState(this.state);
    }

    handleOnTestCaseChanged() {
        if (this.props.onTestCaseChanged) {
            this.props.onTestCaseChanged(this.props.index, this.props.input, this.props.output);
        }
    }

    render() {
        let outputDescription = <small>Define outputs to your problem. </small>;
        if (this.props.selectedLanguage != null && this.props.selectedLanguage.name === "powershell") {
            outputDescription = <small>Define outputs to your problem. You can use <a href="https://github.com/pester/Pester/wiki/Should">Pester Should</a> commands in your expected output. Solution output is available in the <code>$output</code> variable.</small>;
        }

        return(
            <div>
                <div className="form-group">
                    <label htmlFor="input">Input</label>
                    <br />
                    <small>Define the inputs to your problem. Input isn't required. Input should be valid syntax for the language you select.</small>
                    <MonacoEditor contents={this.props.input} onContentChanged={this.onInputContentChanged.bind(this)} />
                </div>
                <div className="form-group">
                    <label htmlFor="output">Expected Output</label>
                    <br />{outputDescription}
                    <MonacoEditor contents={this.props.output} onContentChanged={this.onOutputContentChanged.bind(this)} />
                </div>
            </div>
            );
    }
}