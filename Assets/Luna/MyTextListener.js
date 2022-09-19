

pc.MyTextListener = function () {
    

    this.loadingComplete=function()
    {
      FunPlayableSDK.send('loadingComplete');
     console.log("加载完成");
    }

    this.engagement=function()
    {
   FunPlayableSDK.send('engagement');
     console.log("第一次交互");
    }

    this.tutorial=function()
    {
     FunPlayableSDK.send('tutorial');
     console.log("新手引导");
    }
    this.win=function()
    {
      FunPlayableSDK.send('win');
     console.log("胜利");
    }
      this.lose=function()
    {
      FunPlayableSDK.send('lose');
     console.log("失败");
    }
        this.resize=function()
    {
      FunPlayableSDK.send('resize');
     console.log("屏幕旋转");
    }

    this.goStore=function()
    {
     FunPlayableSDK.send('goStore');
     console.log("商店跳转");
    }

    this.finish=function()
    {
       FunPlayableSDK.send('finish');
     console.log("完成试玩打点");
    }
    this.customfunction = function ( message) {
        FunPlayableSDK.send('custom', message);
        console.log("自定义打点");
    }

    this.addnumber = function () {
        return datanumber;
    }
    this.addnumber1 = function () {
        return datanumber1;
    }
    this.addnumber2 = function () {
        return datanumber2;
    }
    this.isluna = function () {
        return lunaOrHtml;
    }
    this.copyAA = function (message) {
        window.$__toCopy = message;
        console.log("copyAA called")
    }

    document.querySelector('#application-canvas').addEventListener("click", function () {
        console.log("canvas clicked")
        setTimeout(() => {
            if (window.$__toCopy) {
                console.log("content being copied")
                var aux = document.createElement("input");
                aux.setAttribute("value", window.$__toCopy);
                document.body.appendChild(aux);
                aux.select();
                document.execCommand("copy");
                document.body.removeChild(aux);
                window.$__toCopy = "";
            }
        }, 100)
    })
}