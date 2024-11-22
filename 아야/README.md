<div align="center">

# 🍀 아야 - 유사도 기반 질병 분석 시스템

### 주요 기능:
- **증상 분석**: 유사도 계산을 통해 가장 관련 높은 질병을 추천
- **Kaggle 데이터 활용**: Name, Symptoms, Treatments 컬럼 활용
- **Flask 웹 인터페이스**: 사용자가 직접 증상을 입력하고 결과 확인 가능


### 데이터
[kaggle disease and symtoms](https://www.kaggle.com/datasets/aadyasingh55/disease-and-symptoms)
| Code | Name | Symptoms | Treatments |
| --- | --- | --- | --- |
| 코드 | 이름 | 증상 | 치료법 |
|  | 질병 이름 | 질환과 관련된 일반적 증상 | 관리를 위해 권장되는 치료법 또는 요법 |
---

<img src="https://github.com/user-attachments/assets/74e84cd3-2af0-408e-8eeb-753e16dd6187" alt="손목 아야" width="500"/>
<p>손목 아야: 증상 입력 화면</p>

<img src="https://github.com/user-attachments/assets/372c1f50-00c1-434d-91a3-80592d6b51ad" alt="증상 결과" width="500"/>
<p>증상 분석 결과 화면</p>

</div>
---


### 단순 console print

```
환자 데이터: 타자를 칠 때마다 손가락이 불편하고 손목도 아픈 거 같아 🏥

==================================================
🩺 예상 질병 1: 수근관 증후군 (Carpal Tunnel Syndrome) (66.04%)
--------------------------------------------------
📌 관련 증상
    손과 손가락의 따끔 거림, 마비 및 통증

💊 치료 방법
    손목 부목, 반복적 인 손 움직임 방지, 차가운 또는 뜨거운 압축, 온화한 운동, 인체 공학적 조정
==================================================

==================================================
🩺 예상 질병 2: 골절 (Fracture) (61.36%)
--------------------------------------------------
📌 관련 증상
    통증, 붓기, 타박상, 기형, 움직이는 어려움, 기능 상실

💊 치료 방법
    고정화, 수술, 재활
==================================================

==================================================
🩺 예상 질병 3: 손가락으로 부상 (Injury to the Finger) (60.89%)
--------------------------------------------------
📌 관련 증상
    통증, 부기, 타박상, 기형, 제한된 움직임 범위, 손가락을 잡거나 손가락을 사용하는 데 어려움

💊 치료 방법
    라이스 방법 (휴식, 얼음, 압축, 고도), 부서, 진통제, 물리 치료, 수술 (심한 경우)
==================================================
```

```
@misc{bge-m3,
      title={BGE M3-Embedding: Multi-Lingual, Multi-Functionality, Multi-Granularity Text Embeddings Through Self-Knowledge Distillation}, 
      author={Jianlv Chen and Shitao Xiao and Peitian Zhang and Kun Luo and Defu Lian and Zheng Liu},
      year={2024},
      eprint={2402.03216},
      archivePrefix={arXiv},
      primaryClass={cs.CL}
}
```
