#Testing different types of stdin
import sys
import time

#input = sys.stdin.read().split('\n')
#for l in input:
#    print("got " + l + " on stdin")
#    time.sleep(1)

#input = sys.stdin.readline()
#print("got " + input)

for line in sys.stdin:
    print("got " + line + " on stdin")
    time.sleep(1)
