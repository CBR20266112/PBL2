import pandas as pd
from datetime import datetime

file_path = 'c:/Users/vipgo/Dev/PBL2/UPDATE_LOG.xlsx'

try:
    df = pd.read_excel(file_path)
except FileNotFoundError:
    print(f"Error: {file_path} not found.")
    exit(1)

new_row = {
    '버전': 'v1.1',
    '날짜': datetime.now().strftime('%Y-%m-%d'),
    '담당자': 'AI Assistant',
    '수정 내용': '최종 개선 적용: AudioManager.Awake()에서 SfxDatabase 및 ThemeDatabase의 Pre-warming 적용으로 첫 재생 지연(Hitch) 방지'
}

df = pd.concat([df, pd.DataFrame([new_row])], ignore_index=True)
df.to_excel(file_path, index=False)
print("UPDATE_LOG.xlsx 최종 업데이트 완료")
