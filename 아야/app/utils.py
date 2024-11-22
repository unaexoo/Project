import requests
import urllib.parse
import time
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
