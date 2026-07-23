import pandas as pd
from datetime import datetime

file_path = 'c:/Users/vipgo/Dev/PBL2/UPDATE_LOG.xlsx'

try:
    df = pd.read_excel(file_path)
except FileNotFoundError:
    print(f"Error: {file_path} not found.")
    exit(1)

new_row = {
    '버전': 'v1.46.0',
    '날짜': datetime.now().strftime('%Y-%m-%d'),
    '담당자': 'AI Assistant',
    '수정 내용': '사용자 리포트 버그 C# 수정 시도 및 Play Mode 미검증/미해결 상태 기록 갱신'
}

df = pd.concat([df, pd.DataFrame([new_row])], ignore_index=True)
df.to_excel(file_path, index=False)
print("UPDATE_LOG.xlsx 최종 업데이트 완료")
