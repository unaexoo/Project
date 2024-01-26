# FCM 기반 양자화 기법을 이용한 세라믹 영상에서의 결함 검출

## 전처리 방법
### ROI 추출
- 세라믹 영상을 불러온 후 지역 기반 오츠 이진화 적용 -> 양방향 Sobel Mask 적용 후 수평값의 최대값을 활용하여 ROI 영역 추출
![image](https://github.com/unaexoo/Project/assets/142863284/4a20c03e-9cac-49de-b223-6e6171753ef6)

### 잡음 제거
- ROI 영상에 Minimum 필터를 적용하여 객체와 배경 분리 후 가우시안 블러링 적용 -> 히스토그램 기반 스트레칭을 해준 후 Salt and Pepper 잡음 제거에 효과적인 Median 필터 적용
- 아래 영상 순서 : ROI 영상 -> Minimum Filter -> Gaussian Blurring -> 히스토그램 기반 스트레칭 -> Median Filter
![image](https://github.com/unaexoo/Project/assets/142863284/a695329a-f0a6-4408-8248-8356990a6d37)
![image](https://github.com/unaexoo/Project/assets/142863284/5a8f4a60-a55d-43c7-9625-b844bec0765d)

## FCM 기반 양자화 기법
- FCM(Fuzzy C-Means)를 이용하여 양자화 기법을 적용하여 결함 검출

## 결론
- 평균 검출률은 93.94%
