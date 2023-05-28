function clean(s) { return s.textContent.replace(/\s{2,}/g, " ").trim(); }
function RandName() { return (Math.random() + 1).toString(36).substring(3, 7); }

async function ImgToBlob(img) {
    //img.crossOrigin = "Anonymous";
    //img.src += " ";
    let blob;
    try { blob = await fetch(img.src, { mode: "cors" }).then(r => r.blob()); }
    catch (e) { blob = await fetch(img.src, { mode: "no-cors" }).then(r => r.blob()); }
    if (blob.size == 0) return null;
    return blob;
    let canvas = document.createElement('canvas');
    canvas.width = img.clientWidth;
    canvas.height = img.clientHeight;
    let ctx = canvas.getContext('2d');
    ctx.drawImage(img, 0, 0);
    let blobb = await new Promise(resolve => canvas.toBlob(resolve, 'image/png'));
    return blobb;
}
function blobToBase64(blob) {
    return new Promise((resolve, _) => {
        const reader = new FileReader();
        reader.onloadend = () => resolve(reader.result);
        reader.readAsDataURL(blob);
    });
}
if (!Script) var Script = { // Рассчитано, что на сайте уже стоит jQuery lib
    _loadedScripts: [],
    load: function (script) {
        if (this._loadedScripts.indexOf(script) != -1) return false;
        console.log("Подключаем " + script)
        let code = $.ajax({ async: false, url: script }).responseText;
        (window.execScript || window.eval)(code);
        this._loadedScripts.push(script);
        //$$("head").first().insert(Object.extend(
        //  new Element("script", {type: "text/javascript"}), {text: code}
        //));
    }
};
let res_arr = [];
function Loader() {
    for (let table of document.getElementsByClassName("inner")) {
        for (let line of table.children) {
            let [head, body] = line.children;
            let [a, b, c, d, e, f] = head.children;
            let img = a.children[0].children[0];
            let img2 = body.children[1];
            let path = clean(body.children[2]);
            let arr2 = [];
            for (let ch of body.children) {
                if (ch.nodeName != "UL") continue;
                let arr = [];
                for (let ch2 of ch.children) arr.push(clean(ch2));
                arr2.push(arr);
            }
            let res = [img, clean(a), clean(b), clean(c), clean(d), clean(e), clean(f), img2, path, arr2];
            // console.log(JSON.stringify(res));
            res_arr.push(res);
        }
    }
}
async function Imager() {
    let names = {};
    let images = {};
    for (let line of res_arr) for (let num of [0, 7]) {
        let blob = await ImgToBlob(line[num]);
        let data = await blobToBase64(blob);
        let name = names[data];
        if (name == undefined) {
            name = names[data] = RandName();
            images[name] = data;
        }
        line[num] = name;
        if (blob != null) console.log(name + " loaded: " + blob.size + "b.");
        else console.log(name + " cors-error :/");
    }
    let zip = new JSZip();
    zip.file("images.json", JSON.stringify(images));
    zip.file("yeah.json", JSON.stringify(res_arr));
    // console.log(zip);
    zip.generateAsync({ type: "blob" }).then(function (content) {
        let name = location.pathname.split("/");
        name = name[name.length - 2];
        console.log("final:", content, "|", name);
        saveAs(content, "yeah_" + name + ".zip");
    });
}
Script.load("https://raw.githubusercontent.com/VectorASD/Storage/main/jszip.min.js");
Script.load("https://raw.githubusercontent.com/VectorASD/Storage/main/FileSaver.min.js");
if (!!window.JSZip && !!window.saveAs) {
    Loader();
    await Imager();
} else console.log("Ошибка загрузки зипера, либо saveAs функции");