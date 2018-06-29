function initDragAndDropDialog()
/* powers the drag and drop feature */
{           
    document.addEventListener("dragover",function(e){
        e.preventDefault();  
        e.stopPropagation();
    });
    document.addEventListener('dragenter',function(e){
        e.preventDefault();
        e.stopPropagation();
    })
    document.addEventListener('dragleave',function(e){
        e.preventDefault();
        e.stopPropagation();
    })
    document.addEventListener('drop',function(e){
        e.preventDefault();
        let x = document.getElementById("AddorRemove").value;
        if (x==="Remove"){
            document.getElementById("removelocationform").style.display = "block";
        }
        else if (x==="Add"){
            document.getElementById("addlocationform").style.display = "block";
        }
    });
}

function dropHandler(e){
    /* Replaces the block with the file name */
    //console.log("Dropped file name is:", e.dataTransfer.files[0].name);
    document.getElementById("dragDIV").innerHTML = e.dataTransfer.files[0].name; 
    let x = document.getElementById("AddorRemove").value;
    let y = document.getElementById("dragDIV");
}

function onSubmit(){
    console.log("Button has been clicked");
    window.location.replace("onSubmit.html");
    return false;
}

function display() {
    /* no option is selected for the first question, do the following */ 
    let x = document.getElementById("AddorRemove").value;
    let y = document.getElementById("FileLocationSelect").value;
    let z = document.getElementById("FileLocationSelect2").value;

    if (x === ""){
        document.getElementById("dragDIV").style.display = "none";
        document.getElementById("runbutton").style.display = "none";
        document.getElementById("removelocationform").style.display = "none";
        document.getElementById("addlocationform").style.display = "none";
    }
    else if (x === "Remove"){
        document.getElementById("dragDIV").style.display = "block";
        if ((y || z) != ""){
            document.getElementById("runbutton").style.display = "block";
        }else{
            document.getElementById("runbutton").style.display = "none";
        }
        document.getElementById("addlocationform").style.display = "none";
    }
    else if (x === "Add"){
        document.getElementById("dragDIV").style.display = "block";
        if ((y || z) != ""){
            document.getElementById("runbutton").style.display = "block";
        }else{
            document.getElementById("runbutton").style.display = "none";
        }
        document.getElementById("removelocationform").style.display = "none";
    }

    if (y === "Remove"){
        removeFile();             
    }
    else if (y === "Add"){
        addFile();
    }
}
