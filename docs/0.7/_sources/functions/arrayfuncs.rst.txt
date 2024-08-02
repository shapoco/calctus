Array
#################

aggregate(array, func)
*****************

Apply the aggregate function `func(a,b)` for the `array`. ::

    aggregate([1,2,3,4,5],(a,b)=>a+b)
                             //--> 15

all(array)
*****************

Returns true if all `array` elements are true. ::

    all([true,false,true]) //--> false
    all([true,true,true])  //--> true

all(array, func)
*****************

Returns true if tester function `func(x)` returns true for all elements of the `array`. ::

    all([2,3,5,7],isPrime)   //--> true
    all([2,3,5,7,9],isPrime) //--> false

any(array)
*****************

Returns true if at least one element is true. ::

    any([false,false,false]) //--> false
    any([false,true,false])  //--> true

any(array, func)
*****************

Returns true if tester function `func(x)` returns true for at least one element of the `array`. ::

    any([1,2,3,4,5],x=>x%2==0) //--> true
    any([1,3,5,7,9],x=>x%2==0) //--> false

arrayComp(array0, array1)
*****************

Returns true if all elements of the two arrays are the same. ::

    arrayComp([1,2,3], [1,2,3])   //--> true
    arrayComp([1,2,3], [1,2,3,4]) //--> false
    arrayComp([1,2,3], [1,2,10])  //--> false
    arrayComp([], [])             //--> true
    arrayComp([], [1,2,3])        //--> false

concat(array0, array1)
*****************

Concatenate array0 and array1. ::

    concat([1,2,3],[4,5,6]) //--> [1, 2, 3, 4, 5, 6]

contains(array, *val)
*****************

Returns whether the `array` contains `val`. ::

    contains([8,1,4,0,7,2,5,4,3],4) //--> true
    contains([8,1,4,0,7,2,5,4,3],6) //--> false

count(array, func)
*****************

Count specific elements in the `array` using a tester function `func(x)`. ::

    count(0..10,x=>x%3==0) //--> 4

except(array0, array1)
*****************

Returns the difference set of the two arrays. ::

    except(0..10,[1,4,7,10,13,16]) //--> [0, 2, 3, 5, 6, 8, 9]

extend(array, func, count)
*****************

Extends the `array` using converter function `func(array)`. ::

    extend([0,1],a=>a[-2]+a[-1],5) //--> [0, 1, 1, 2, 3, 5, 8]

filter(array, func)
*****************

Filter the `array` using a tester function `func(x)`. ::

    filter(0..10,x=>x%3==0) //--> [0, 3, 6, 9]

indexOf(array, *val)
*****************

Returns the index of the first element in the `array` whose value matches `val`. ::

    indexOf([8,1,4,0,7,2,5,4,3],4) //--> 2
    indexOf([8,1,4,0,7,2,5,4,3],6) //--> -1

intersect(array0, array1)
*****************

Returns the product set of the two arrays. ::

    intersect(0..10,[1,4,7,10,13,16])
                             //--> [1, 4, 7]

lastIndexOf(array, *val)
*****************

Returns the index of the last element in the `array` whose value matches `val`. ::

    lastIndexOf([8,1,4,0,7,2,5,4,3],4)
                              //--> 7
    lastIndexOf([8,1,4,0,7,2,5,4,3],6)
                              //--> -1

len(array)
*****************

Length of `array` ::

    len([])            //--> 0
    len([12,34,56,78]) //--> 4
    len("Hello")       //--> 5

map(array, func)
*****************

Map the `array` using a converter function `func(x)`. ::

    map(0..5,x=>x*x) //--> [0, 1, 4, 9, 16]

range(start, stop)
*****************

Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop`. ::

    range(3,9) //--> [3, 4, 5, 6, 7, 8]

range(start, stop, step)
*****************

Returns an array consists of sequence of numbers greater than or equal to `start` and less than `stop` with common difference `step`. ::

    range(3,9,2) //--> [3, 5, 7]

rangeInclusive(start, stop)
*****************

Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop`. ::

    rangeInclusive(3,9) //--> [3, 4, 5, 6, 7, 8, 9]

rangeInclusive(start, stop, step)
*****************

Returns an array consists of sequence of numbers greater than or equal to `start` and less than or equal to `stop` with common difference `step`. ::

    rangeInclusive(3,9,2) //--> [3, 5, 7, 9]

reverseArray(array)
*****************

Reverses the order of elements of `array` ::

    reverseArray([12,3,4567,890]) //--> [890, 4567, 3, 12]

sort(array)
*****************

Sort the `array`. ::

    sort([4,3,5,1,2]) //--> [1, 2, 3, 4, 5]

sort(array, func)
*****************

Sort the `array` using a evaluator function `func(x)`. ::

    sort([14,23,35,41,52],x=>x%10) //--> [41, 52, 23, 14, 35]

union(array0, array1)
*****************

Returns the union of the two arrays. ::

    union(0..10,[1,4,7,10,13,16]) //--> [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13, 16]

unique(array)
*****************

Returns an array of unique elements. ::

    unique([3,0,1,2,2,1,3,4]) //--> [3, 0, 1, 2, 4]

unique(array, func)
*****************

Return unique elements using evaluator function `func(x)`. ::

    unique([13,20,31,42,52,61,73,84],(a,b)=>a%10==b%10)
                                           //--> [13, 20, 31, 42, 84]


