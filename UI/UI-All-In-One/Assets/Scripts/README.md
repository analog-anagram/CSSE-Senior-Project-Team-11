ScaleToUser
To make this script work, put ths script on the item you want to scale and set the following:

userCamera: The camera that we use to do calculations of distance from the item.
pivotPoint: The object that will be used to find disance between user and item for scale.
isCenter: Determines if this item is the item we use for determining distance for scaling.
ScaleDist: Distance at which we expect the item to be the original scale, any item further away
than this will scale at the far scale rate, any item closer will scale at the nearScale rate.
farScale/nearScale: determines the rate of linear change in size based on distance. 
a value of 1 will make the item stay the same size, more than 1 will make the item grow,
a value of less will make the items shrink as you move away but at a lower speed.
