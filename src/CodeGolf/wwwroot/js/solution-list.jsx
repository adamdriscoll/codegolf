class SolutionRow extends React.Component {
    render() {
        var deleteLink = null;

        if (this.props.isSolutionOwner) {
            deleteLink = <a href={this.props.deleteSolutionUrl}><i className="fa fa-trash-o fa-lg"></i></a>;
        }

        var authIcon = null;
        if (this.props.authorAuthType === "github") {
            authIcon = <i className="fa fa-github fa-lg"></i>;
        }

        return(<div>
                    <tr>
                            <td>
                                <VoteButtons />
                            </td>
                            <td><a href={this.props.authorProfileUrl}>{authIcon} {this.props.author}</a></td>
                            <td><span className="badge">{this.props.solutionLength}</span></td>
                            <td>{this.props.solutionDate}</td>
                            <td>{deleteLink}</td>
                    </tr>
                    //<tr style="display: none;" id="@(item.Id)container">
                    //    <td colspan="5">
                    //        <div id="@item.Id" style="height: 200px; border: 1px solid grey"></div>
                    //    </td>
                    //</tr>
                </div>);
    }
}

class SolutionTable extends React.Component {
    constructor() {
        super();

        this.state = {
            solutions: []
        };
    }

    renderSolution(solution) {
        return <SolutionRow isSolutionOwner={solution.isSolutionOwner}
                             deleteSolutionUrl={solution.deleteSolutionUrl}
                             authorAuthType={solution.authorAuthType}
                             authorProfileUrl={solution.authorProfileUrl}
                             author={solution.author}
                             solutionLength={solution.solutionLength}
                             solutionDate={solution.solutionDate}/>;
    }

    componentWillMount() {
        var self = this;
        $.get(this.props.solutionDataUrl,
            function(data) {
                self.state.solutions = data;
                self.setState(self.state);
            });
    }

    render() {
        const solutionRows = this.state.solutions.map(solution => this.renderSolution(solution));

        return (<table className="table">
                    <tr>
                        <th>Crowd Appeal</th>
                        <th>Player</th>
                        <th>Score</th>
                        <th>Date Played</th>
                        <th></th>
                    </tr>
                    {solutionRows}
                </table>);
    }
}