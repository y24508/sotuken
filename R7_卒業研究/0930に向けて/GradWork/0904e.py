import streamlit as st
from streamlit_option_menu import option_menu
import pandas as pd

# サイドバーのメニュー作成
with st.sidebar:
    selected = option_menu(
        "Top Page",  # メニューのタイトル
        ["入力フォーム", "編集画面", "閲覧・検索画面"],  # メニュー項目
        icons=["pen", "person", "search"],  # アイコン
        menu_icon="cast",  # メニューのアイコン
        default_index=0  # デフォルトで選択される項目
    )

# 入力フォーム画面
if selected == "入力フォーム":
    st.title("入力フォーム")
    st.write("ようこそ過去卒業研究閲覧・検索画面へ！")

# 編集画面
elif selected == "編集画面":
    st.title("編集画面")
    st.write("ここに編集できます")

# 閲覧・検索画面
# 閲覧・検索画面
elif selected == "閲覧・検索画面":
    st.title("過去卒業研究閲覧・検索画面")

    # Excelファイルのアップロード
    uploaded_file = st.file_uploader("過去卒業研究のデータをアップロードしてください（Excel形式）", type=["xlsx"])

    if uploaded_file is not None:
        # データの読み込み
        df = pd.read_excel(uploaded_file)
        if "年" in df.columns:
            df["年"] = df["年"].astype(str)

        # データフレーム表示
        st.write("アップロードされたデータ：")
        st.dataframe(df.reset_index(drop=True), use_container_width=True)

        # 指導教員による絞り込み
        if "指導教員" in df.columns:
         st.write("### 指導教員による絞り込み")

        # 「すべて表示」オプションを追加
        teachers = df["指導教員"].dropna().unique()
        teacher_options = ["すべて表示"] + sorted(teachers.tolist())
        selected_teacher = st.selectbox("指導教員を選んでください", teacher_options)

        # 条件に応じてデータを表示
        if selected_teacher == "すべて表示":
            st.write(f"すべての卒業研究一覧（{len(df)} 件）")
            st.dataframe(df.reset_index(drop=True), use_container_width=True)
        else:
            teacher_filtered_df = df[df["指導教員"] == selected_teacher]
            count = len(teacher_filtered_df)
            st.write(f"選択された指導教員：{selected_teacher} の卒業研究一覧（{count} 件）")
            st.dataframe(teacher_filtered_df.reset_index(drop=True), use_container_width=True)
