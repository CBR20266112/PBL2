import pandas as pd
from datetime import datetime

file_path = 'c:/Users/vipgo/Dev/PBL2/UPDATE_LOG.xlsx'

try:
    df = pd.read_excel(file_path)
except FileNotFoundError:
    print(f"Error: {file_path} not found.")
    exit(1)

new_row = {
    '버전': 'v0.9',
    '날짜': datetime.now().strftime('%Y-%m-%d'),
    '담당자': 'AI Assistant',
    '수정 내용': 'Phase 2 구현: SFX Object Pool 추가, Polyphony 동시 재생 제어, Pitch/Volume 랜덤화 및 Priority 속성을 위한 SfxDatabase 도입'
}

df = pd.concat([df, pd.DataFrame([new_row])], ignore_index=True)
df.to_excel(file_path, index=False)
print("UPDATE_LOG.xlsx Phase 2 업데이트 완료")
