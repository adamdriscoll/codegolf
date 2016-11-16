class MonacoEditor extends React.Component {
    constructor() {
        super();

        this.state = {
            editor: null,
            initialized: false
        }
    }
    loadData() {
        var self = this;
        $.get(this.dataUrl, (data) => self.createEditor(data.content, data.language, self.props.readonly));
    }

    createEditor(contents, language, readonly) {
        this.state.editor = monaco.editor.create(this.editorContainer,
        {
            value: contents,
            language: language,
            readOnly: readonly
        });

        this.setState(this.state);

        this.state.editor.getModel().onDidChangeContent(this.handleOnContentChanged.bind(this));
    }

    handleOnContentChanged() {
        var content = this.state.editor.getModel().getLinesContent().join('\n');

        if (this.props.onContentChanged) {
            this.props.onContentChanged(content);
        }
    }

    componentDidUpdate() {
        if (this.state.initialized) {
            return;
        }

        this.state.initialized = true;
        this.setState(this.state);

        const self = this;
        require.config({ paths: { 'vs': "/lib/monaco-editor/" } });
        require(["vs/editor/editor.main"],
            function () {
                self.createEditor(self.props.contents, self.props.language, self.props.readonly);
            });
    }

    componentDidMount() {
        if (this.props.waitForContent && (this.props.contents == null || this.props.contents === "")) {
            return;
        }

        this.state.initialized = true;
        this.setState(this.state);

        const self = this;
        require.config({ paths: { 'vs': "/lib/monaco-editor/" } });
        require(["vs/editor/editor.main"],
            function () {
                if (self.props.dataUrl) {
                    self.loadData();
                } else {
                    self.createEditor(self.props.contents, self.props.language, self.props.readonly);
                }
            });
    }

    render() {
        return (<div ref={(domNode) => this.editorContainer = domNode} className="monaco-container"></div>);
    }
}