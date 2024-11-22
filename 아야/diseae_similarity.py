import os
os.environ['HUGGINGFACEHUB_API_TOKEN'] = ''

from sentence_transformers import SentenceTransformer, util
import numpy as np
import pandas as pd
import requests
import urllib.parse
import torch
from flask import Flask, request, render_template, jsonify
import time

app = Flask(__name__)
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

csv_path = r'C:\disease_similianity\data\Diseases_Symptoms.csv'
data = pd.read_csv(csv_path)
data = data.drop(columns=['Code'])

# 증상 텍스트 준비
corpus = data['Symptoms'].fillna("").tolist()

embedding_model_id = "BAAI/bge-m3"
sentence_model = SentenceTransformer(embedding_model_id).to(device)

def calc_similarity(query, top_k=5):
    query_embedding = sentence_model.encode(query, convert_to_tensor=True).to(device)
    corpus_embedding = sentence_model.encode(corpus, convert_to_tensor=True).to(device)

    cos_scores = util.pytorch_cos_sim(query_embedding, corpus_embedding)[0].to(device)
    cos_scores_cpu = cos_scores.cpu().numpy()

    top_k = min(top_k, len(cos_scores_cpu))
    top_res = np.argpartition(-cos_scores_cpu, range(top_k))[:top_k]
    top_res = top_res[np.argsort(-cos_scores_cpu[top_res])]

    res = []
    for idx in top_res:
        sentence = corpus[idx].strip()
        res.append((idx, sentence, cos_scores_cpu[idx]))
    return res

def translate(text, source_lang="en", target_lang="ko",delay = 1):
    encoded_text = urllib.parse.quote(text)
    url = f"https://lingva.ml/api/v1/{source_lang}/{target_lang}/{encoded_text}"
    
    try:
        response = requests.get(url)
        response.raise_for_status()
        time.sleep(delay) 
        return response.json()["translation"]
    except requests.exceptions.RequestException as e:
        print(f"번역 API 오류: {e}")
        return text

def translate_ko(text, delay=1):
    return translate(text, source_lang="en", target_lang="ko",delay=delay)

patient_input = "복통이 있고 설사를 계속해요. "
patient_en = translate(patient_input, source_lang="ko", target_lang="en")

top_res = calc_similarity(patient_en, top_k=3)

print(f"환자 데이터: {patient_input} 🏥")
for i, (idx, _, score) in enumerate(top_res):
    disease_name = data.iloc[idx]["Name"]
    symptoms = data.iloc[idx]["Symptoms"]
    treatments = data.iloc[idx]["Treatments"]
    
    # 번역 (영어 -> 한국어)
    disease_name_ko = translate_ko(disease_name)
    symptoms_ko = translate_ko(symptoms)
    treatments_ko = translate_ko(treatments)
    
    # 출력
    print("\n" + "=" * 50)
    print(f"🩺 예상 질병 {i + 1}: {disease_name_ko} ({score * 100:.2f}%)")
    print("-" * 50)
    print(f"📌 관련 증상\n    {symptoms_ko}\n")
    print(f"💊 치료 방법\n    {treatments_ko}")
    print("=" * 50)


