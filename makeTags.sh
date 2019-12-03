 for i in {L,R}; do for j in {A,E,W,I,L,M,R,T}; do if [ $j = A ] || [ $j == E ]
 || [ $j == W ]; then echo $i$j; else for k in {1..3}; do echo $i$j$k; done; fi; done; done
