class [ComponentName] extends React.Component {
    constructor(props) {
        super(props);

        // Connect this component to the back-end view model.
        this.vm = dotnetify.react.connect("[ComponentName]", this);

        // Set up function to dispatch state to the back-end.
        this.dispatchState = state => this.vm.$dispatch(state);

        // The VM's initial state was generated server-side and included with the JSX.
        this.state = {  };
    }
    componentWillUnmount() {
        this.vm.$destroy();
    }
    render() {
        return (
            [ComponentContent]
        );
    }
}