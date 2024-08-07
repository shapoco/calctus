Array
#################

aggregate(array, func)
*****************

Apply the aggregate function `func(a,b)` for the `array`.

all(array)
*****************

Returns true if all `array` elements are true.

all(array, func)
*****************

Returns true if tester function `func(x)` returns true for all elements of the `array`.

any(array)
*****************

Returns true if at least one element is true.

any(array, func)
*****************

Returns true if tester function `func(x)` returns true for at least one element of the `array`.

arrayComp(array0, array1)
*****************

Returns true if all elements of the two arrays are the same.

concat(array0, array1)
*****************

Concatenate array0 and array1.

contains(array, *val)
*****************

Returns whether the `array` contains `val`.

count(array, func)
*****************

Count specific elements in the `array` using a tester function `func(x)`.

except(array0, array1)
*****************

Returns the difference set of the two arrays.

extend(array, func, count)
*****************

Extends the `array` using converter function `func(array)`.

filter(array, func)
*****************

Filter the `array` using a tester function `func(x)`.

indexOf(array, *val)
*****************

Returns the index of the first element in the `array` whose value matches `val`.

intersect(array0, array1)
*****************

Returns the product set of the two arrays.

lastIndexOf(array, *val)
*****************

Returns the index of the last element in the `array` whose value matches `val`.

len(array)
*****************

Length of `array`

map(array, func)
*****************

Map the `array` using a converter function `func(x)`.

range(start, stop)
*****************

Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop`.

range(start, stop, step)
*****************

Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop` with common difference `step`.

rangeInclusive(start, stop)
*****************

Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop`.

rangeInclusive(start, stop, step)
*****************

Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop` with common difference `step`.

reverseArray(array)
*****************

Reverses the order of elements of `array`

sort(array)
*****************

Sort the `array`.

sort(array, func)
*****************

Sort the `array` using a evaluator function `func(x)`.

union(array0, array1)
*****************

Returns the union of the two arrays.

unique(array)
*****************

Returns an array of unique elements.

unique(array, func)
*****************

Return unique elements using evaluator function `func(x)`.


