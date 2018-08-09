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
                notify: true,
            },
            fileList: {
                type: Array,
                value: [],
                notify: true
            },
            dropMenus: {
                type: Array,
                notify: true,
                value: []
            },
            dropHandler: {
                type: Array,
                notify: true,
                value: []
            },
            hiddenReset: {
                type: Boolean,
                value: true,
            },
            hiddenAdd: {
                type: Boolean,
                value: true,
            },
            hiddenSubmit: {
                type: Boolean,
                value: true,
            },
            selectedIndex: {
                type: String,
                notify: true,
            }
        };
    }

    _onLoad(e) {
        var arrayLength = e.detail.response.length;

        // below makes an array of arrays with the full file path location at first index
        for (var i = 0; i < arrayLength; i++) {
            var res = e.detail.response[i].split('\\');
            // this will change, we just use a test directory right now and 4 is the constant with it
            var temp = res.slice(4);
            this.files.splice(0, 0, temp); // add the first thing in the directory here
            this.files[0].splice(0, 0, (e.detail.response[i])); // set the full path string at the beginning of the array
        }

        var temp_array = [];
        for (var i = 0; i < arrayLength; i++) {
            if (temp_array.indexOf(this.files[i][1]) === -1){
            //if (!temp_array.includes(this.files[i][1])) {
                temp_array.push((this.files[i][1]));
            }
        }
        var tempArr = this.dropMenus.slice();
        tempArr.push(temp_array);
        this.dropMenus = tempArr;
    }

    _dropdownMenu(e) {
        if (this.dropHandler.indexOf(e.model.index) === -1){
        //if (!this.dropHandler.includes(e.model.index)) {
            this.dropHandler.push(e.model.index);
        }
        else {
            var removal, listboxclear = "";
            for (var i = e.model.index + 1; i < this.dropMenus.length; i++) {
                removal = "#dropdown" + i;
                this.querySelector(removal)._setSelectedItem(null);
                listboxclear = '#thelistbox' + i;
                this.querySelector(listboxclear).selected = -1;
            }
            this.splice('dropMenus', e.model.index + 1, this.dropMenus.length);
            this.dropHandler.splice(e.model.index + 1, this.dropHandler.length);
            this.hiddenAdd = true;
        }

        var temp_array = [];
        var arrayLength = this.files.length;
        this.selectedIndex = e.target.selectedItem.innerText;
        this.selectedIndex = this.selectedIndex.trim();

        for (var i = 0; i < arrayLength; i++) {
            if (this.files[i][e.model.index + 1] == this.selectedIndex) {
                if (temp_array.indexOf(this.files[i][e.model.index + 2]) === -1 && (this.files[i][e.model.index + 2] != null)) {
                    temp_array.push((this.files[i][e.model.index + 2]));
                }
            }
        }

        if (temp_array.length) {
            var tempArr = this.dropMenus.slice();
            tempArr.push(temp_array);
            this.dropMenus = tempArr;
        }
        else {
            this.hiddenAdd = !this.hiddenAdd;
        }
        if (this.hiddenReset) { this.hiddenReset = !this.hiddenReset }
    }

    _itemAdded(e) {

        var currentSelection = "";
        var finalPath = [];

        for (var i = 0; i < this.dropHandler.length; i++) {
            currentSelection = "#dropdown" + i;
            currentSelection = this.querySelector(currentSelection).value;

            if (i + 1 != this.dropHandler.length) { finalPath += (currentSelection + '\\') }
            else { finalPath += currentSelection };
        }


        if (this.fileList.indexOf(finalPath) === -1) { this.fileList.push(finalPath); }
        else { this.$['file-toast'].text = "File is already added"; this.$['file-toast'].open(); }

        if (!this.submitHidden) {
            var temp_array = [];
            var tempArr = this.fileList.slice();
            this.fileList = tempArr;
        }

        this.hiddenAdd = true;
        this.hiddenSubmit = false;
        this._reset(e);
    }

    _reset(e) {
        this.splice('dropMenus', 1, this.dropMenus.length);
        this.querySelector('#dropdown0')._setSelectedItem(null);
        this.querySelector('#thelistbox0').selected = -1;

        this.hiddenReset = !this.hiddenReset;
        this.hiddenAdd = true;
    }

    _deleteEntry(e) {
        this.splice('fileList', e.model.index, 1);
        if (this.fileList.length == 0) { this.hiddenSubmit = true; }
    }

    _onSubmit() {
        this.$.submitajax.generateRequest();
    }

    _afterSubmit(e) {
        this.hiddenSubmit = !this.hiddenSubmit;
        this.hiddenBox = !this.hiddenBox;
        this.fileList = [];
        this.$['file-toast'].open();
    }

    _fileerror(e) {
        this.$['file-toast'].text = e.detail.error.message;
        this.$['file-toast'].open();
    }

    attached() {
        //super.attached();
        if (this.guruSite.Servicing) {
            this.querySelector('.filemanagerlogoimage').src = "../../Images/Servicing/Filemanager-logo.svg";
            this.querySelector('#filemanagerheaderimage').src = "../../Images/Servicing/Filemanager-header.svg";
            this.customStyle['--primary-color'] = '#8DC63F';
            this.updateStyles();
        }
    }
}

Polymer(FileManager);