public class TrackInfo {

    string name;
    string path;
    string type;

    public TrackInfo() {
        this.name = null;
        this.path = null;
        this.type = null;
    }

    public TrackInfo(string name, string path, string type) {
        this.name = name;
        this.path = path;
        this.type = type;
    }

    public string GetName() {
        return this.name;
    }

    public string GetPath() {
        return this.path;
    }

    public string GetFileType() {
        return this.type;
    }
		

}
