

function circle(r) {
    return parseFloat(r) * Math.PI * 2.0;


}
console.log(circle(5.138))

console.log("************************")

function circle(r) {

    return parseInt(r) * Math.PI * 2.0;

}
console.log(circle(5.138))

console.log("************************")

 const getRectArea=function(height, width) {
    return height * width;
}
 console.log(getRectArea(3, 4));
 console.log("************************")


 const getget = (height, width)=>{
    return height * width;

 
}
console.log(getget(3, 4));

console.log("************************")
const computeRectArea = new Function('height','width',' return height * width')
console.log(computeRectArea(9,7))