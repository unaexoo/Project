# -*- coding: utf-8 -*-
"""
Created on Tue Jun 20 14:43:55 2023

@author: Admin
"""
import time
import keyboard
import gymnasium as gym

#observation, info = env.reset(seed=41)
#observation, info = env.reset()

# print(observation)
# print()
# print(info)
# print()

env = gym.make("CartPole-v1", render_mode="human")
Gene=[0.4807, 0.5293, 0.5864, 0.5172, 0.9198, 0.6007, 0.1591, -0.1088, 0.2632, 0.3793, -0.5387, -0.56, 0.8413, 0.0634, 0.4579, 0.7645, 0.8589, -0.5434]

INPUT = 4
HIDDEN = 3
OUTPUT = 2
NumOfGens = 64
GeneLength = INPUT*HIDDEN + HIDDEN*OUTPUT
BGenes=[]

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
        
while(True):
    observation, info = env.reset() # observation : 현재 게임에서 4가지 상태값

    count = 0
    sm = 0.0
    mmax=-5
    mmin=5
    while(True):

       n = normal(observation)
       action = decide_output(n,Gene)
       observation, reward, terminated, truncated, info = env.step(action)
       
       #print(observation)
       #print(reward)
       
       sm += reward
       
       # if reward == 100: 
       #     print(action)
       #     print()
       #     print(observation)
       #     print()
       #     print(reward)
       #     print()
       #     print(terminated)
       #     print()
       #     print(truncated)
       #     print()
       #     print(info)
       #     print()
       #     print("Success!")
       #     break
       
       #x = input()
       if observation[0] > mmax:
          mmax = observation[0]
      
       if(observation[0]<mmin):
          mmin = observation[0]
       if terminated or truncated:
          y = 100-20*(mmax-mmin)
          print("Final Fitness:", sm+y, mmax-mmin)
          break
      
    if truncated:
        print("success!")
       #print(count)

#env.close()


