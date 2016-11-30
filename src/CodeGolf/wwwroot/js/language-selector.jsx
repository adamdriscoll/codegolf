class LanguageSelector extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            language: "",
            nonDefaultLanguageName: "",
            nonDefaultLanguage: false,
            selectedLanguage: null,
            availableLanguages: []
        }
    }

    componentWillMount() {
        const self = this;

        $.get(this.props.languagesUrl,
             function (data) {
                 self.state.availableLanguages = data;
                 self.setState(self.state);

                 self.state.language = self.props.language;
                 self.setState(self.state);
             });
    }

    onLanguageChange(event) {
        this.state.language = event.target.value;

        this.state.selectedLanguage = null;
        this.state.availableLanguages.forEach(lang => {
            if (lang.name === event.target.value) {
                this.state.selectedLanguage = lang;
            }
        });

        this.state.nonDefaultLanguage = this.state.language === "Other";
        this.setState(this.state);

        this.onSelectedLanguageChanged();
    }

    onNonDefaultLanguageChange(e) {
        this.state.nonDefaultLanguageName = e.target.value;
        this.setState(this.state);

        this.onSelectedLanguageChanged();
    }

    onSelectedLanguageChanged() {
        let languageName = this.state.language;
        if (this.state.nonDefaultLanguage) {
            languageName = this.state.nonDefaultLanguageName;
        }

        this.props.onSelectedLanguageChanged(this.state.selectedLanguage, languageName);
    }

    renderLanguageOptions(language) {
        return <option value={language.name} key={language.name}>{language.name}</option>;
    }

    render() {
        const languageOptions = this.state.availableLanguages.map((lang) => this.renderLanguageOptions(lang));

        let otherLanguageText = null;
        if (this.state.nonDefaultLanguage) {
            otherLanguageText = <div className="form-group">
                                    <label htmlFor="name">Other Language</label>
                                   <input type="text" className="form-control" placeholder="Other Language" value={this.state.nonDefaultLanguageName} onChange={this.onNonDefaultLanguageChange.bind(this)} />
                               </div>;
        }

        return <div className="row form-group">
                    <div className="col-md-3">
                        <label htmlFor="name">Language</label>
                        <select className="form-control" value={this.state.language} onChange={this.onLanguageChange.bind(this)}>
                            {languageOptions}
                        </select>
                    </div>
                    <div className="col-md-3">
                        {otherLanguageText}
                    </div>
                </div>;
    }
}