
function ShaderToy(shaderCodeEditor)
{
    this.mCreated = false;
    this.mShaderCodeEditor = shaderCodeEditor;
    this.mShaderCodeTextArea = document.getElementById("myShaderCode");
    this.mGLContext = null;
    this.mHttpReq = null;
    this.mEffect = null;
    this.mTo = null;
    this.mTOffset = 0;
    this.mCanvas = null;
    this.mFpsFrame = 0;
    this.mFpsTo = null;
    this.mIsPaused = false;
    this.mForceFrame = false;

    this.mCanvas = document.getElementById("demogl");
    this.mHttpReq = new XMLHttpRequest();
    this.mTo = (new Date()).getTime();
    this.mTf = 0;
    this.mFpsTo = this.mTo;
    this.mMouseOriX = -1;
    this.mMouseOriY = -1;
    this.mMousePosX = -1;
    this.mMousePosY = -1;

    try
    {
        this.mGLContext = this.mCanvas.getContext("experimental-webgl");
    }
    catch (e)
    {
        this.mGLContext = null;
    }

    if( this.mGLContext==null )
    {
        var div = document.createElement("div");
        div.innerHTML = "This demo requires a WebGL-enabled browser.";
        var canvasParent = this.mCanvas.parentNode;
        canvasParent.replaceChild(div, this.mCanvas);
        return;
    }

    var me = this;

    this.mCanvas.onmousedown = function(ev)
    {
        var obj = me.mCanvas; var xo = yo = 0; do { xo += obj.offsetLeft; yo += obj.offsetTop; }while (obj = obj.offsetParent);
        me.mMouseOriX = ev.clientX - xo;
        me.mMouseOriY = ev.clientY - yo;
    }
    this.mCanvas.onmousemove = function(ev)
    {
        if(me.mMouseOriX>0)
        {
            var obj = me.mCanvas; var xo = yo = 0; do { xo += obj.offsetLeft; yo += obj.offsetTop; }while (obj = obj.offsetParent);
            me.mMousePosX = ev.pageX - xo;
            me.mMousePosY = ev.pageY - yo;
            //document.getElementById("kkkk").value = me.mMousePosX + ", " + me.mMousePosY;
        }
    }
    this.mCanvas.onmouseup = function(ev)
    {
        me.mMouseOriX = -1;
        me.mMouseOriY = -1;
    }

    //document.getElementById("myShaderCode").onKeyPress = function(ev)
    me.AltIsDown = false;
    this.mShaderCodeTextArea.onkeyup = function(ev) { if( ev.keyCode==0x12 ) me.AltIsDown=false; }
    this.mShaderCodeTextArea.onkeydown = function(ev)
    {
        if( ev.keyCode==0x12 ) me.AltIsDown=true;
        if( ev.keyCode==0x0d && me.AltIsDown )
        {
          me.newShader();
        }
    }

    this.mEffect = new Effect(this.mGLContext, this.mCanvas.width, this.mCanvas.height);
    this.mCreated = true;
}

ShaderToy.prototype.startRendering = function()
{
    function renderLoop2(me)
    {
        if( me.mIsPaused && !me.mForceFrame )
        {
            setTimeout(renderLoop2, 20, me );
            return;
        }

        me.mForceFrame = false;
        var time = (new Date()).getTime();
        var ltime = me.mTOffset + time - me.mTo;
        if( me.mIsPaused ) ltime = me.mTf;

        me.mEffect.Paint(ltime/1000.0, me.mMouseOriX, me.mMouseOriY, me.mMousePosX, me.mMousePosY);
        me.mGLContext.flush();

        me.mFpsFrame++;

        document.getElementById("myTime").innerHTML = (ltime/1000.0).toFixed(2);
        if( (time-me.mFpsTo)>1000 )
        {
            var ffps = 1000.0 * me.mFpsFrame / (time-me.mFpsTo);
            document.getElementById("myFramerate").innerHTML = ffps.toFixed(1) + " fps";
            me.mFpsFrame = 0;
            me.mFpsTo = time;
        }

        setTimeout(renderLoop2, 0, me );
    }
    setTimeout(renderLoop2, 0, this );
}

//ShaderToy.prototype.resize = function(xres, yres)
ShaderToy.prototype.resize = function()
{
    var xres = document.getElementById("myXResolution").value;
    var yres = document.getElementById("myYResolution").value;
    this.mCanvas.setAttribute("width", xres);
    this.mCanvas.setAttribute("height", yres);
    this.mCanvas.width = xres;
    this.mCanvas.height = yres;

    this.mEffect.SetSize(xres,yres);
}
//---------------------------------

ShaderToy.prototype.pauseTime = function()
{
    var time = (new Date()).getTime();
    if( !this.mIsPaused )
    {
        document.getElementById("myPauseButton").src="./Shader Toy_files/play.png";
        this.mTf = this.mTOffset + time - this.mTo;
        this.mIsPaused = true;
    }
    else
    {
        document.getElementById("myPauseButton").src="pause.png";
        this.mTOffset = this.mTf;
        this.mTo = time;
        this.mIsPaused = false;
    }
}

ShaderToy.prototype.resetTime = function()
{
    this.mTOffset = 0;
    this.mTo = (new Date()).getTime();
    this.mTf = 0;
    this.mFpsTo = this.mTo;
    this.mFpsFrame = 0;
    this.mForceFrame = true;
}


ShaderToy.prototype.newShader = function()
{
    var value;
    if( this.mShaderCodeEditor==null )
        value = this.mShaderCodeTextArea.value;
    else
        value = this.mShaderCodeEditor.getCode();

    var result = this.mEffect.NewShader(value);
    var errorTxtBox = document.getElementById("errorTxtBox");
    errorTxtBox.value = result;
    this.mForceFrame = true;
}

ShaderToy.prototype.setTexture = function(slot,txt)
{
    this.mEffect.NewTexture(slot,txt);
    this.mForceFrame = true;
}

ShaderToy.prototype.setShader = function(url)
{
    this.mHttpReq.open("GET", url, false);
    this.mHttpReq.send(null);

    var text = this.mHttpReq.responseText;
    if( this.mShaderCodeEditor==null )
    {
        this.mShaderCodeTextArea.value = text;
    }
    else
    {
        this.mShaderCodeEditor.setCode( text );
        this.mShaderCodeEditor.syntaxHighlight('init');
    }

    var result = this.mEffect.NewShader(text);
    var errorTxtBox = document.getElementById("errorTxtBox");
    errorTxtBox.value = result;
}

ShaderToy.prototype.newScript = function(url)
{
    var shaderURL = null;
    var texture0URL = null;
    var texture1URL = null;
    var texture2URL = null;
    var texture3URL = null;
    var comments = null;

    this.mHttpReq.open("GET", url, false);

    try
    {
        this.mHttpReq.send(null);
    }
    catch(e)
    {
        alert( "could not load script " + url + ": " + e );
        return false;
    }

    var xml = this.mHttpReq.responseXML;

    var ele_shadertoy = xml.getElementsByTagName("shadertoy");
    if( ele_shadertoy.length != 1 )
    {
        alert( "could not load script" );
        return false;
    }
    var version = ele_shadertoy[0].attributes["version"].value;
    if( version != "0.1" )
    {
        alert( "could not load script" );
        return false;
    }

    var ele_options = xml.getElementsByTagName("options");
    if( ele_options.length != 1 )
    {
        alert( "could not load script" );
        return false;
    }
    var ele_resolution = ele_options[0].getElementsByTagName("resolution");
    if( ele_resolution.length != 1 )
    {
        alert( "could not load script" );
        return false;
    }
    var xres = ele_resolution[0].attributes["xres"].value;
    var yres = ele_resolution[0].attributes["yres"].value;

    var ele_shader = ele_shadertoy[0].getElementsByTagName("shader");
    if( ele_shader.length != 1 )
    {
        alert( "could not load script" );
        return false;
    }

    shaderURL = ele_shader[0].attributes["src"].value;

    var ele_textures = ele_shadertoy[0].getElementsByTagName("textures");
    if( ele_textures.length != 1 )
    {
        alert( "could not load script" );
        return false;
    }

    var texture = ele_textures[0].getElementsByTagName("texture");
    for (i = 0; i < texture.length; i++)
    {
        var lid = texture[i].attributes["id"].value;
        var lsrc = texture[i].attributes["src"].value;
        if( lid=="0" ) { texture0URL = lsrc; }
        if( lid=="1" ) { texture1URL = lsrc; }
        if( lid=="2" ) { texture2URL = lsrc; }
        if( lid=="3" ) { texture3URL = lsrc; }
    }

    var ele_info = ele_shadertoy[0].getElementsByTagName("info");
    if( ele_info.length != 1 )
    {
        alert( "could not load script" );
        return false;
    }
    var infoName = ele_info[0].attributes["name"].value;
    var infoAuthor = ele_info[0].attributes["author"].value;
    var infoYear = ele_info[0].attributes["date"].value;
    var infoLink = ele_info[0].attributes["link"].value;

    var ele_comments = ele_info[0].getElementsByTagName("comments");
    comments = ele_comments[0].firstChild.data;

    //------------------------

//    document.getElementById("myScript").value = shaderURL;
    document.getElementById("myTexture0").value = texture0URL;
    document.getElementById("myTexture1").value = texture1URL;
    document.getElementById("myTexture2").value = texture2URL;
    document.getElementById("myTexture3").value = texture3URL;
    document.getElementById("myInfo").innerHTML = "'" + infoName + "' by " + infoAuthor + " (" + infoYear + ")";
    document.getElementById("myInfo").href = infoLink;
    document.getElementById("myComments").value = comments;
//    document.getElementById("myXResolution").value = xres;
//    document.getElementById("myYResolution").value = yres;

    //------------------------

    //this.resize();
    this.setShader(shaderURL);
    this.setTexture(0,texture0URL);
    this.setTexture(1,texture1URL);
    this.setTexture(2,texture2URL);
    this.setTexture(3,texture3URL);

    this.resetTime();
    this.mForceFrame = true;

    return true;
}
