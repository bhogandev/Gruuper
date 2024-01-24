

class Timeline extends React.Component {
    static displayName = Timeline.name;
  
      constructor(props) {
          super(props);
  
          this.state = {
              posts: [],
              updateTl: false,
              page: 1
          };
  
          this.tlupdate = this.tlupdate.bind(this);
      }
  
      //NEEDS REFACTORING
      infiniteScroll = () => {
              // End of the document reached?
              if (window.innerHeight + document.documentElement.scrollTop
              === document.documentElement.offsetHeight){
               
                 let newPage = this.state.page;
                 newPage++;
                  this.setState({
                       page: newPage
                  });
                 this.GetTL();
                 }
      }
  
      tlupdate() {
          this.setState({ updateTl: true });
      }
  
      
  
      componentDidMount() {
          this.GetTL();
          window.addEventListener('scroll', this.infiniteScroll)
      }
  
      componentDidUpdate() {
          if (this.state.updateTl) {
              this.GetTL();
              this.setState({ updateTl: false });
          }
          window.addEventListener('scroll', this.infiniteScroll)
      }
  
  
      render() {
          if (this.state.posts != null || this.state.posts.length > 0) {
              return (
                  <Container style={{ textAlign: "left" }}>
                      posts go here
                      <h1>{this.state.posts}</h1>
                  </Container>
              );
          }
          return (
              <div><p>Loading...</p></div>
              )
    }
  }
  
  export default Timeline;
  