class MonacoEditor extends React.Component {
    constructor() {
        super();

        this.state = {
            editor: null
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

        this.state.editor.getModel().onDidChangeContent(this.handleOnContentChanged.bind(this));
    }

    handleOnContentChanged() {
        var content = this.state.editor.getModel().getLinesContent().join('\n');

        if (this.props.onContentChanged) {
            this.props.onCOntentChanged(content);
        }
    }

    componentDidMount() {
        const self = this;
        require.config({ paths: { 'vs': "/lib/monaco-editor/" } });
        require(["vs/editor/editor.main"],
            function () {
                if (self.props.dataUrl) {
                    loadData();
                } else {
                    createEditor(self.props.contents, self.props.langauge, self.props.readonly);
                }
            });
    }

    render() {
        return (<div ref={(domNode) => this.editorContainer = domNode}></div>);
    }
}