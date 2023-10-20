# -*- coding: utf-8 -*-
"""
Created on Tue Jun 20 14:43:55 2023

@author: Admin
"""
import time
import keyboard
import gymnasium as gym
import numpy as np
import random

#observation, info = env.reset(seed=41)
#observation, info = env.reset()

# print(observation)
# print()
# print(info)
# print()

env = gym.make("CartPole-v1", render_mode="human")


while(True):
    observation, info = env.reset() # observation : 현재 게임에서 4가지 상태값

    count = 0
    sm = 0.0

    while(True):
       count += 1

       time.sleep(0.15)
       
       #action = env.action_space.sample()  # this is where you would insert your policy
       
       action = count % 2 # 키보드 입력이 없으면 0, 1을 번갈아 가지도록 설정
       
       if keyboard.is_pressed("right arrow"):
           action = 1
    
       if keyboard.is_pressed("left arrow"):
           action = 0
       
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
      
       if terminated or truncated:
          #print("reward(sum):", sm)
          print("Final Fitness:", sm)
          break
      
    if truncated:
        print("success!")
       #print(count)

#env.close()


