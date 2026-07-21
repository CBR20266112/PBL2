import pandas as pd
from datetime import datetime

file_path = 'c:/Users/vipgo/Dev/PBL2/UPDATE_LOG.xlsx'

# 엑셀 파일 읽기
try:
    df = pd.read_excel(file_path)
except FileNotFoundError:
    print(f"Error: {file_path} not found.")
    exit(1)

# 새 행 데이터
new_row = {
    '버전': 'v0.8',
    '날짜': datetime.now().strftime('%Y-%m-%d'),
    '담당자': 'AI Assistant',
    '수정 내용': 'Phase 1 구현: AudioManager 확장 (CrossFade, Ambience, AudioMixer 연동) 및 SettingsManager 업데이트'
}

# DataFrame에 새 행 추가
df = pd.concat([df, pd.DataFrame([new_row])], ignore_index=True)

# 엑셀 파일 저장
df.to_excel(file_path, index=False)
print("UPDATE_LOG.xlsx 업데이트 완료")
