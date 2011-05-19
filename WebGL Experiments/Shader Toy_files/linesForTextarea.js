function addLinesToTextArea(id)
{
    var ta = document.getElementById(id);
    var el = document.getElementById('linesColumn');

    el.style.overflow='hidden';

    var lineObj = document.createElement('DIV');
    lineObj.style.position = 'relative';
    lineObj.style.top = '-1px';
    lineObj.style.textAlign = 'right';
    lineObj.style.height = ta.offsetHeight;

    lineObj.style.fontSize = ta.style.fontSize;
    lineObj.style.fontFamily = ta.style.fontFamily;
    lineObj.className='lineObj';
    el.appendChild(lineObj);

    var string = ''; for(var no=1;no<2000;no++) string = string + no + '<br>';
    lineObj.innerHTML = string;

    ta.onkeydown = function() { positionLineObj(lineObj,ta); };
    ta.onmousedown = function() { positionLineObj(lineObj,ta); };
    ta.onscroll = function() { positionLineObj(lineObj,ta); };
    ta.onblur = function() { positionLineObj(lineObj,ta); };
    ta.onfocus = function() { positionLineObj(lineObj,ta); };
    ta.onmouseover = function() { positionLineObj(lineObj,ta); };
}

function positionLineObj(obj,ta)
{
    obj.style.top = (-1-ta.scrollTop) + 'px';
}
