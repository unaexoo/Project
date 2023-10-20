# -*- coding: utf-8 -*-
"""
Created on Tue Jun 20 14:43:55 2023

@author: Admin
"""
import time
import keyboard
import gymnasium as gym
import random

#observation, info = env.reset(seed=41)
#observation, info = env.reset()

# print(observation)
# print()
# print(info)
# print()

env = gym.make("CartPole-v1", render_mode="human")

INPUT = 4
HIDDEN = 3
OUTPUT = 2
NumOfGens = 64
GeneLength = INPUT*HIDDEN + HIDDEN*OUTPUT

BGenes=[]
AGenes=[]

Boundary=30
Elite=2

Epoch_ten = 10 
tonament=NumOfGens//10
# 입력값들을 -1.0~ 1.0 정규화하기 위한 기준값
PosNorm = 4.8   #0
SpeedNorm = 1.7 #1,3
AngleNorm = 0.418 #2

def normal(observation):
    tmp = []
    tmp.append(observation[0]/PosNorm)
    tmp.append(observation[1]/SpeedNorm)
    tmp.append(observation[2]/AngleNorm)
    tmp.append(observation[3]/SpeedNorm)
    return tmp


def makeInitGenes():
    global BGenes
    for i in range(NumOfGens):
        a=[]
        for j in range(GeneLength):
           a.append(round(random.uniform(-1.0, 1.0),4))
        BGenes.append(a)
        

def decide_output(n,Gens) :
    hidden = [] 
    output=[]

    for i in range(HIDDEN):
        sum=0.0
        for j in range(INPUT):
            sum = sum + Gens[i*INPUT+j]*n[j]

        hidden.append(sum)
        
    for i in range(HIDDEN) :
        if hidden[i]>1.0:
            hidden[i]=1.0
        elif hidden[i]< -1.0:
            hidden[i]=-1.0
            
            
    for i in range(OUTPUT):
        sum=0.0
        for j in range(HIDDEN):
            sum = sum + Gens[HIDDEN*INPUT+i*HIDDEN+j]*hidden[j]
        output.append(sum)
        
    tmp = max(output)
    idx = output.index(tmp)
    return idx
        
        
def clacFitness() : 
    for i in range(NumOfGens):
       
        total_reward=0.0
        
        count=0
        fail=0
        while(True) : 
            sm = 0.0
            mmax=-5
            mmin=5

            observation, info = env.reset()
            
            while(True):
                n = normal(observation)
                action = decide_output(n,BGenes[i])
                observation, reward, terminated, truncated, info = env.step(action)
           
                if observation[0] > mmax:
                        mmax = observation[0]
               
                if(observation[0]<mmin):
                       mmin = observation[0]
                   
                sm += reward
                """if terminated or truncated:
                    y = 100-20*(mmax-mmin)
                    BGenes[i].append(sm+y)
                    print(i, "Final Fitness:", sm+y, mmax-mmin)
                    
                if truncated : 
                    print("success!")
                """

                if terminated or truncated:
                    y = 100-20*(mmax-mmin)
                      #print("reward(sum):", sm)
                    if truncated :
                        count +=1
                            
                        total_reward = total_reward+sm+y
                    else :
                        fail= 1
                        total_reward = total_reward+sm+y
                  
                    break
                
            if fail==1 or count==5 :
                break
                          
        BGenes[i].append(total_reward)        
        print(i, "Final Fitness:" ,count, round(total_reward,3), round(mmax-mmin),3)    

def selectandCrossover():
    global BGenes
    global AGenes
    count=Elite
    for i in range(Elite):
        AGenes.append(BGenes[i][:GeneLength])
    

    while(True):
       mmax= -100000
       p1=0
       for i in range(tonament) :
           idx= random.randint(0, NumOfGens-1)
           if BGenes[idx][GeneLength] > mmax : 
               mmax = BGenes[idx][GeneLength]
               p1=idx
     
    
       mmax=-100000
       p2=0
       
       for i in range(tonament) :
            idx= random.randint(0, NumOfGens-1)
        
            if BGenes[idx][GeneLength] > mmax:
                mmax = BGenes[idx][GeneLength]
                p2=idx
            
           
       print(p1, p2)
       cut = random.sample(range(GeneLength), 2)
       cut1 = int(cut[0])
       cut2= int(cut[1])
        
       if cut1>cut2:
            tmp = cut2
            cut2=cut1
            cut1=tmp
            
       print(cut1,cut2)
       c1 = BGenes[p1][:cut1]+BGenes[p2][cut1:cut2] + BGenes[p1][cut2:GeneLength]
       c2 = BGenes[p2][:cut1]+BGenes[p1][cut1:cut2] + BGenes[p2][cut2:GeneLength]
        
       AGenes.append(c1)
       AGenes.append(c2)
        
       count+=2
        
       if count == NumOfGens:
           break
    
def mutate():
    # 엘리트는 건들면 안 됨 0,1번 인덱스는 건들지 말기 -> 62개(2~63까지)
    # 0~100의 랜덤 구하기 rand<30이면 -> 전역변수로 두기
    # 반복하면서 유전자 18개 값이 if rand<30 : true -> 인덱스 사이의 랜덤값을 구하기 -> 유전자의 길이 미만 randrange(길이)
    # 유전자 초기 만들 때 쓴 랜덤 사용해서 랜덤으로 찾아진 인덱스에 값을 바꿔치기    
    for i in range(2,63):
        n = random.randint(1,101)
        if n < Boundary:
            idx = random.randrange(GeneLength)
            AGenes[i][idx] = round(random.uniform(-1.0, 1.0),4)
        
makeInitGenes()
GENRATION=50
count=1

while(True):
    clacFitness()
    
    # 적응도값 내림차순 정렬
    BGenes = sorted(BGenes,key = lambda x : -x[GeneLength])
    # print(BGenes)
    
    print("Best : ", BGenes[0][GeneLength])
        
    print("Best : ", BGenes[1][GeneLength])
    
    #for i in range(GeneLength):
    #   print(BGenes[0][i], end =', ')
        
        
    # 1등,2등 파일 저장
    file = open('gen.txt','a')
    file.write('fitness : ')
    file.write(str(count))
    file.write('\n')
    file.write('1 : ')
    file.write(str(BGenes[0]))
    file.write('\n')
    file.write('2 : ')
    file.write(str(BGenes[1]))
    file.write('\n')
    file.close()
    
    # 진화세대 일정 이상되면 break
    if count==GENRATION:
        break

     # 선택교차 - AGenes
    selectandCrossover() 
     # Bgnes로 Agens 세트 생성
     # # 돌연변이
    mutate() # AGens 유전들에 일정 확률로 돌연변이

    BGenes = AGenes[:]  # 유전자 전체 복사
    AGenes=[]
    count = count+1
