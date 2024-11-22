import os
os.environ['HUGGINGFACEHUB_API_TOKEN'] = ''

from sentence_transformers import SentenceTransformer, util
import torch
import numpy as np

# GPU 설정
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

# 모델 로드
embedding_model_id = "sentence-transformers/all-mpnet-base-v2"
sentence_model = SentenceTransformer(embedding_model_id).to(device)

def calc_similarity(query, data, top_k=5):
    corpus = data['Symptoms'].fillna("").tolist()
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
