class FileManager {
    beforeRegister() {
        this.is = 'file-manager';

        this.properties = {
            filemanagerheaderimage: {
                type: String
            },
            files: {
                type: Array,
                notify: true,
                value: []
            },
            guruSite: {
                type: Object,
                value: () => { return document.getElementById('guruAppHost').site; },
                notify: true
            },
            fileList: {
                type: Array,
                value: [],
                notify: true
            },
            hiddenReset: {
                type: Boolean,
                value: true
            },
            hiddenSubmit: {
                type: Boolean,
                value: true
            },
            searchQuery: {
                type: String,
                notify: true,
                observer: '_filterFiles'
            },
            filteredFiles: {
                type: Array,
                notify: true,
                value: []
            }
        };
    }

    _onLoad(e) {
        this.set("files", e.detail.response.sort().reverse());
        this._filterFiles("");
    }

    itemclicked(e) {
        this.hiddenSubmit = false;
        let input = e.currentTarget.parentElement.innerText.trim();
        let index = this.files.indexOf(input);
        this.push('fileList', this.files.splice(index, 1)[0]);
        this.splice('filteredFiles', this.filteredFiles.indexOf(input), 1);
        if (this.hiddenReset) { this.hiddenReset = !this.hiddenReset;}
    }

    _filterFiles(input) {
        this.set('filteredFiles', this.files.filter(x=> x.toLowerCase().indexOf(input.toLowerCase()) != -1));
    }

    _reset(e) {
        this.files = this.files.concat(this.fileList);
        this.filteredFiles = this.files;
        this.fileList = [];
        this.$['input-box'].value = "";
        this.hiddenReset = !this.hiddenReset;
        this.hiddenSubmit = !this.hiddenSubmit;
        this._filterFiles("");

    }

    _deleteEntry(e) {
        let input = e.currentTarget.parentElement.innerText.trim();
        this.files.push(input);
        this.splice('fileList', this.fileList.indexOf(input), 1);
        if (!this.fileList.length) {
            this.hiddenReset = !this.hiddenReset;
            this.hiddenSubmit = !this.hiddenSubmit;
        }

        this._filterFiles(this.$["input-box"].value);
    }

    _onSubmit() {
        this.$.submitajax.generateRequest();
    }

    _afterSubmit(e) {
        this.hiddenSubmit = !this.hiddenSubmit;
        this.fileList =[];
        this.$['file-toast'].text = "File(s) Deleted";
        this.$['file-toast'].open();
    }

    _fileerror(e) {
        this.$['file-toast'].text = e.detail.error.message;
        this.$['file-toast'].open();
    }

    attached() {
        if (this.guruSite.Servicing) {
            this.querySelector('.filemanagerlogoimage').src = "../../Images/Servicing/Filemanager-logo.svg";
            this.querySelector('#filemanagerheaderimage').src = "../../Images/Servicing/Filemanager-header.svg";
            this.customStyle['--primary-color'] = '#8DC63F';
            this.updateStyles();
        }
    }
}

Polymer(FileManager);
