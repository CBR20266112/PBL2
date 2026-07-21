import pandas as pd
from datetime import datetime

file_path = 'c:/Users/vipgo/Dev/PBL2/UPDATE_LOG.xlsx'

try:
    df = pd.read_excel(file_path)
except FileNotFoundError:
    print(f"Error: {file_path} not found.")
    exit(1)

new_row = {
    '버전': 'v1.0',
    '날짜': datetime.now().strftime('%Y-%m-%d'),
    '담당자': 'AI Assistant',
    '수정 내용': 'Phase 3 구현: ThemeAudioData 및 ThemeDatabase 도입, SettingsManager 하드코딩(switch문) 제거, 데이터 기반 국가 테마 구조 확립'
}

df = pd.concat([df, pd.DataFrame([new_row])], ignore_index=True)
df.to_excel(file_path, index=False)
print("UPDATE_LOG.xlsx Phase 3 업데이트 완료")
