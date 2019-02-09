# Example prints primes from 0-100 one every second
import time
primes = []
for i in range(2,100):
    ip = [1 if i % p == 0 else 0 for p in primes]
    if sum(ip) == 0:
        primes.append(i)
for p in primes:
    time.sleep(1)
    print(p)
