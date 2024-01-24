export function setPropVal(prop, val) {
    val = val.trim();

    this.setState({
        [prop]: val
    });
};

export function renderUIErrors(){
    if (document.getElementById("errors") != null) {
        this.state.errors.forEach(
            x => document.getElementById("errors").innerHTML = (x["Description"])
        )
    }
}

export function StringIsEmpty(string){
    return string === '' ? true : false;
}

export function CUIToJson(cuiTemplate, cuiObject){
    var jsonConstruct = "";


}