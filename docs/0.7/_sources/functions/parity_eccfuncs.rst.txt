Parity/Ecc
#################

eccDec(b, ecc, x)
*****************

Checks ECC code (`b`: data width, `ecc`: ECC code, `x`: data). Returns: 0 = no error, positive value = position of 1-bit error, negative value = 2-bit error.

eccEnc(b, *x)
*****************

Generates ECC code (`b`: data width, `x`: data).

eccWidth(*b)
*****************

Width of ECC for `b`-bit data.

oddParity(*x)
*****************

Odd parity of `x`.

xorReduce(*x)
*****************

Reduction XOR of `x` (Same as even parity).


