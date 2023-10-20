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

while(True) :
    observation, info = env.reset()
    
    count = 0
    sm = 0.0

    while(True):
       count += 1
       
       #action = env.action_space.sample()  # this is where you would insert your policy
       action = 0;
       time.sleep(0.15)
           
       if keyboard.is_pressed("down arrow"):
           action = 2
    
       if keyboard.is_pressed("right arrow"):
           action = 1
    
       if keyboard.is_pressed("left arrow"):
           action = 3
       
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
