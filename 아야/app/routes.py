from flask import Blueprint, render_template, request, jsonify, redirect, url_for, session
from .model import calc_similarity
from .utils import translate, translate_ko
import pandas as pd
import logging
import json  # JSON 처리 모듈

# 로그 설정
logging.basicConfig(
    level=logging.DEBUG,
    format='%(asctime)s - %(levelname)s - %(message)s',
    datefmt='%Y-%m-%d %H:%M:%S'
)

main = Blueprint('main', __name__)

# 데이터 로드
csv_path = r'C:\disease_similianity\data\Diseases_Symptoms.csv'
try:
    data = pd.read_csv(csv_path).drop(columns=['Code'])
    required_columns = {'Name', 'Symptoms', 'Treatments'}
    if not required_columns.issubset(data.columns):
        raise ValueError(f"CSV file must contain columns: {required_columns}")
except Exception as e:
    logging.error(f"Failed to load data from {csv_path}: {e}")
    data = None


def translate_with_fallback(text, target_language='ko', source_language='en'):
    """
    번역을 시도하며 실패 시 원문을 반환.
    """
    try:
        return translate(text, source_lang=source_language, target_lang=target_language)
    except Exception as e:
        logging.warning(f"Translation failed for text '{text}': {e}")
        return text


@main.route('/')
def home():
    return render_template('index.html')


@main.route('/predict', methods=['POST'])
def predict():
    global data
    try:
        if not isinstance(data, pd.DataFrame) or data.empty:
            return jsonify({"error": "Data is not loaded or is empty"}), 500

        request_data = request.get_json()
        if not request_data:
            return jsonify({"error": "No JSON payload provided"}), 400

        # 사용자 입력 처리
        patient_input = request_data.get('patient_input') or request_data.get('symptoms')
        if not patient_input or not patient_input.strip():
            return jsonify({"error": "No input provided or input is empty"}), 400

        patient_input = patient_input.strip()

        # 세션에 사용자 입력 저장
        session['patient_input'] = patient_input

        # 입력 번역 (한->영)
        patient_en = translate_with_fallback(patient_input, target_language='en', source_language='ko')

        # 유사도 계산
        top_res = calc_similarity(patient_en, data, top_k=3)

        # 결과 데이터 생성
        matched_symptoms = []
        generated_response = []
        for idx, _, score in top_res:
            disease_row = data.iloc[idx]
            disease_name_ko = translate_with_fallback(disease_row["Name"])
            symptoms_ko = translate_with_fallback(disease_row["Symptoms"])
            treatments_ko = translate_with_fallback(disease_row["Treatments"])

            matched_symptoms.append(disease_name_ko)
            generated_response.append({
                "disease_name": disease_name_ko,
                "symptoms": symptoms_ko,
                "treatments": treatments_ko,
                "confidence": f"{score * 100:.2f}%"
            })

        # 결과 데이터를 세션에 저장
        session['matched_symptoms'] = matched_symptoms
        session['generated_response'] = json.dumps(generated_response)

        return redirect(url_for('main.result'))

    except Exception as e:
        logging.exception("Error during prediction")
        return jsonify({"error": f"Unexpected server error: {str(e)}"}), 500


@main.route('/result')
def result():
    # 세션에서 데이터 가져오기
    patient_input = session.get('patient_input', '증상이 입력되지 않았습니다.')
    matched_symptoms = session.get('matched_symptoms', [])
    generated_response = json.loads(session.get('generated_response', '[]'))  # JSON 역직렬화

    # 디버깅 로그
    print("Patient Input:", patient_input)
    print("Matched Symptoms:", matched_symptoms)
    print("Generated Response:", generated_response)

    return render_template(
        'result.html',
        patient_input=patient_input,  # 사용자 입력 전달
        matched_symptoms=matched_symptoms,
        generated_response=generated_response
    )


