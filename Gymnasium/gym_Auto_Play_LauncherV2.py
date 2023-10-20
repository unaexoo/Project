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

env = gym.make("LunarLander-v2", render_mode="human")


INPUT = 8
HIDDEN = 6
OUTPUT = 4
NumOfGens = 64
GeneLength = INPUT*HIDDEN + HIDDEN*OUTPUT
Elite=2

Gene = [0.0632, -0.1188, -0.6112, -0.7398, 0.3915, 0.8283, -0.3188, -0.7197, -0.5845, -0.7837, -0.4836, 0.8511, 0.4552, 0.6018, 0.791, -0.5263, 0.8795, -0.6317, -0.975, -0.3758, -0.3163, -0.3165, 0.2721, 0.9078, 0.8128, 0.1643, 0.7763, -0.5133, -0.6544, -0.4865, 0.3432, -0.368, 0.3373, 0.985, -0.8043, 0.2168, 0.9138, 0.6943, -0.1203, 0.0471, 0.7651, -0.5553, -0.8264, -0.8403, -0.5196, -0.1481, -0.6804, 0.8991, -0.9781, -0.0586, 0.6011, 0.3689, 0.4847, -0.928, -0.1124, 0.1457, 0.77, 0.2474, 0.7619, 0.3702, 0.7393, -0.8618, 0.095, -0.9357, -0.2151, 0.532, -0.785, 0.4818, 0.2336, -0.1137, -0.3623, -0.1109]

BGenes=[]
AGenes=[]
Boundary=30
tonament=NumOfGens//10
# 입력값들을 -1.0~ 1.0 정규화하기 위한 기준값
# 1.5, 0,1
# 3.1415927, 2 ,3
# 5, 
PosNorm = 1.5 #0
SpeedNorm = 5#1,3
AngleNorm = 3.1415927 #2

def normal(observation): # 8개
    tmp = []
    tmp.append(observation[0]/PosNorm)
    tmp.append(observation[1]/PosNorm)
    tmp.append(observation[2]/SpeedNorm)
    tmp.append(observation[3]/SpeedNorm)
    tmp.append(observation[4]/AngleNorm)
    tmp.append(observation[5]/SpeedNorm)
    tmp.append(observation[6])
    tmp.append(observation[7])
    return tmp


def decide_output(observe,Gens) :
    inputs = normal(observe)
    hidden = [] 
    output=[]

    for i in range(HIDDEN):
        sum=0.0
        for j in range(INPUT):
            sum = sum + Gens[i*INPUT+j]*inputs[j]

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
while(True) :
    observation, info = env.reset()
    count = 0
    sm = 0.0

    while(True):
       count += 1
       action = decide_output(observation,Gene)
       observation, reward, terminated, truncated, info = env.step(action)
           
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
      
       if terminated or truncated:   # rerminated : 달 착륙선의 움직임이 없을때 True,  trucated : 달 착륙선이 착률과 충돌하지 않고 계속 공중에 떠있을 때 (1000번) Ture
          if reward == 100:
              print("Success!")
          elif reward == -100:
              print("Fail!")
          print("Final Fitness:", sm)
          break
          
    print(count)

#env.close()
