import streamlit as st

st.set_page_config(
    page_title="卒業研究アプリ",
    layout="wide",  # ← wideにするとスマホでも横幅を自動調整してくれる
    initial_sidebar_state="collapsed"  # ← サイドバーをデフォルトで閉じる
)

from streamlit_option_menu import option_menu
import pandas as pd
import os
import matplotlib.pyplot as plt
from matplotlib import rcParams
import json

# --- 日本語フォント設定 ---
rcParams["font.family"] = "Yu Gothic"
rcParams["axes.unicode_minus"] = False

st.markdown("""
<style>
@media (max-width: 821px) {
    .stButton>button {
        width: 100%;
        font-size: 1.1em;
        padding: 12px;
    }
    .stTextInput>div>div>input {
        font-size: 1.1em;
        widrh: 100%;
    }
    .stSelectbox>div>div>select {
        font-size: 1.1em;
        width: 100%;
    }
    .block-container {
        padding-top: 2.5rem;
        padding-left: 1rem;
        padding-right: 1rem;
        padding-bottom: 1rem;
    }
    .center-title {
        font-size: 26px !important;
    }    
    h3 {
        font-size: 19px !important;  /* 小さくしたいサイズに調整 */
        text-align: center !important;
    }
    .hide-mobile {
        display: none !important;
    }
    .table-cell, .table-header {
        padding-top: 2px !important;
        padding-bottom: 5px !important;
        margin-top: 0 !important;
        margin-bottom: 0 !important;
    }
}
</style>
""", unsafe_allow_html=True)

KEYWORD_FILE = "keyword_history.json"

# --- 履歴読み込み ---
def load_keyword_history():
    if os.path.exists(KEYWORD_FILE):
        with open(KEYWORD_FILE, "r", encoding="utf-8") as f:
            return json.load(f)
    else:
        return []

# --- 履歴保存 ---
def save_keyword_history(history):
    with open(KEYWORD_FILE, "w", encoding="utf-8") as f:
        json.dump(history, f, ensure_ascii=False, indent=2)

# st.set_page_config(layout="wide")

# --- データファイルのパス ---
DATA_FILE = "過去卒業研究データ.xlsx"

# --- データ読込関数 ---
@st.cache_data
def load_data():
    if os.path.exists(DATA_FILE):
        return pd.read_excel(DATA_FILE)
    else:
        return pd.DataFrame(columns=["ID", "名前", "テーマ", "教員"])

# --- データ保存関数 ---
def save_data(df):
    df.to_excel(DATA_FILE, index=False)
    st.success("Excelファイルを更新しました。")

# --- サイドバーのメニュー ---
with st.sidebar:
    selected = option_menu(
        "Top Page",
        ["追加", "編集", "閲覧・検索", "資料","使い方"],
        icons=["pen", "person", "search", "file-earmark-text"],
        menu_icon="cast",
        default_index=2
    )

# --- 入力フォーム画面 ---
if selected == "追加":
    st.markdown(
    """
    <style>
    .center-title {
        text-align: center;
        font-size: 56px;
        font-weight: bold;
        
        margin-bottom: 20px;
    }
    </style>
    <div class="center-title">研究データ追加ページ</div>
    """,
    unsafe_allow_html=True
)

    st.write("以下の項目を入力して新しい研究データを追加してください。")

    # --- セッションステートの初期化 ---
    if "df" not in st.session_state:
        if os.path.exists(DATA_FILE):
            df = pd.read_excel(DATA_FILE)
            if "年" in df.columns:
                df["年"] = df["年"].astype(str)
            st.session_state.df = df
        else:
            st.session_state.df = pd.DataFrame(columns=["年", "タイトル", "指導教員", "言語", "ジャンル", "概要"])

    df = st.session_state.df

    # --- 入力フォーム作成 ---
    with st.form("input_form", clear_on_submit=True):
        col1, col2 = st.columns(2)
        with col1:
            year = st.selectbox("年を選択", [str(y) for y in range(2010, 2051)])
            teacher = st.text_input("指導教員")
            language = st.text_input("使用言語（カンマ区切りで複数可）")

        with col2:
            title = st.text_input("研究タイトル")
            genre = st.selectbox(
                "ジャンルを選択",
                [
                    "ゲーム・エンタメ系",
                    "画像処理・AI・Deep Learning",
                    "Web・アプリ開発系",
                    "IoT・センサー・ハード連携",
                    "システム・運用・自動化",
                ]
            )
            summary = st.text_area("概要", height=120)

        submitted = st.form_submit_button("追加")

        # --- データ追加処理 ---
        if submitted:
            if not year or not title:
                st.warning("年とタイトルは必須項目です。")
            else:
                new_row = pd.DataFrame({
                    "年": [year],
                    "タイトル": [title],
                    "指導教員": [teacher],
                    "言語": [language],
                    "ジャンル": [genre],
                    "概要": [summary]
                })
                st.session_state.df = pd.concat([st.session_state.df, new_row], ignore_index=True)
                st.session_state.df.to_excel(DATA_FILE, index=False)
                st.success(f"『{title}』のデータを追加しました！")

    st.subheader("現在登録されているデータ")
    st.dataframe(df.drop("ID",axis=1),hide_index=True,use_container_width=True)

# --- 編集画面 ---
elif selected == "編集":
    st.cache_data.clear()
    st.markdown(
    """
    <style>
    .center-title {
        text-align: center;
        font-size: 56px;
        font-weight: bold;
        margin-bottom: 20px;
    }
    </style>
    <div class="center-title">研究データ編集ページ</div>
    """,
    unsafe_allow_html=True
)
    # --- セッション変数の初期化 ---
    if "delete_message" not in st.session_state:
        st.session_state.delete_message = ""
    if "confirm_delete" not in st.session_state:
        st.session_state.confirm_delete = False
    if "delete_target" not in st.session_state:
        st.session_state.delete_target = None   

    # --- データ読み込み ---
    df = load_data()
    if "年" in df.columns:
        df["年"] = df["年"].astype(str).str.replace(",", "")

    # ID列がなければ自動生成
    if "ID" not in df.columns:
        df.insert(0, "ID", range(1, len(df) + 1))

    if df.empty:
        st.info("まだデータがありません。")
    else:
        # --- データ選択 ---
        st.subheader("編集または削除するデータを選択")
        selected_id = st.selectbox("タイトルを選択", df["タイトル"])
        selected_row = df[df["タイトル"] == selected_id].iloc[0]

        # --- 編集フォーム ---
        with st.form("edit_form"):
            new_year = st.text_input("年", selected_row["年"])
            new_title = st.text_input("タイトル", selected_row["タイトル"])
            new_teacher = st.text_input("指導教員", selected_row["指導教員"])
            new_lang = st.text_input("言語", selected_row["言語"])
            new_genre = st.text_input("ジャンル", selected_row["ジャンル"])
            new_summary = st.text_area("概要", selected_row["概要"])

            col1, col2 = st.columns(2)
            with col1:
                update_btn = st.form_submit_button("📝 更新する")
            with col2:
                delete_btn = st.form_submit_button("🗑 削除する")

        # --- 更新処理 ---
        if update_btn:
            df.loc[df["タイトル"] == selected_id, ["年","タイトル","指導教員","言語","ジャンル","概要"]] = \
                [new_year, new_title, new_teacher, new_lang, new_genre, new_summary]
            save_data(df)
            st.success("データを更新しました")
            st.rerun()

        # --- 削除処理 ---
        if delete_btn:
            st.session_state.delete_target = selected_id  # 削除対象タイトルを記録
            st.session_state.confirm_delete = True  # 確認モードに変更
            st.rerun()

        # --- 確認モード ---
        if st.session_state.confirm_delete:
            target = st.session_state.delete_target
            st.warning(f"『{selected_id}』を本当に削除しますか？")
            col1, col2 = st.columns(2)
            with col1:
                yes = st.button("✅ はい、削除します")
            with col2:
                no = st.button("❌ いいえ、やめます")

            if yes:
                df = df[df["タイトル"] != st.session_state.delete_target].reset_index(drop=True)
                df["ID"] = range(1, len(df) + 1)  # ※再採番が不要ならこの行は削除してOK
                save_data(df)
                st.session_state.delete_message = f"『{target}』を削除しました"
                st.session_state.confirm_delete = False
                st.session_state.delete_target = None
                st.rerun()  # 削除後に画面更新（残す）

            if no:
                st.session_state.confirm_delete = False
                st.session_state.delete_target = None
                st.info("削除をキャンセルしました")
                st.rerun()  # キャンセル後に警告を消すため再描画（残す）

    # --- 削除完了メッセージ表示 ---
    if st.session_state.delete_message:
        st.success(st.session_state.delete_message)
        st.session_state.delete_message = ""

    # --- 現在のデータを表示 ---
    st.subheader("📋 現在のデータ")
    st.data_editor(df.drop("ID", axis=1), hide_index=True, use_container_width=True)
# --- 閲覧・検索画面 ---
        # --- 閲覧・検索・グラフ切替画面 ---
elif selected == "閲覧・検索":
    st.markdown(
    """
    <style>
    .center-title {
        text-align: center;
        font-size: 56px;
        font-weight: bold;
        
        margin-bottom: 20px;
    }
    </style>
    <div class="center-title">研究データ閲覧・検索ページ</div>
    """,
    unsafe_allow_html=True
)

    if "df" not in st.session_state:
        if os.path.exists(DATA_FILE):
            df = pd.read_excel(DATA_FILE)
            if "年" in df.columns:
                df["年"] = df["年"].astype(str)
            st.session_state.df = df
        else:
            st.session_state.df = None

    df = st.session_state.df

    if df is not None:

        # --- セッションステート初期化 ---
        if "selected_filters" not in st.session_state:
            st.session_state.selected_filters = {}
        if "logic_type" not in st.session_state:
            st.session_state.logic_type = "AND条件"

        # --- マッピング辞書 ---
        language_mapping = {
            "C": "C系", "C#": "C系", "C++": "C系", "C#": "C系",
            "Python": "Python", "python": "Python", "Python3": "Python","Python3.9": "Python","Python(PyCharm)": "Python","microPython": "Python",                                   
            "Java": "Java", "JavaScript": "JavaScript", "JS": "JavaScript","javascript":"JavaScript",
            "html/css": "html/css", "html":"html/css","css":"html/css","HTML":"html/css","CSS":"html/css",
            "ExcelVBA": "Excel VBA","Excel VBA": "Excel VBA","VBA": "Excel VBA",
            "PHP": "PHP","php":"PHP"
        }

        genre_mapping = {
            "ゲーム": "ゲーム・エンタメ系", "ゲーム・エンタメ系": "ゲーム・エンタメ系",
            "Game": "ゲーム・エンタメ系", "Unity": "ゲーム・エンタメ系",
            "Web": "Web・アプリ開発系", "Web・アプリ開発系": "Web・アプリ開発系",
            "ウェブ": "Web・アプリ開発系", "HTML": "Web・アプリ開発系",
            "CSS": "Web・アプリ開発系", "JavaScript": "Web・アプリ開発系",
            "画像処理・AI・Deep Learning": "画像処理・AI・Deep Learning",
            "人工知能": "画像処理・AI・Deep Learning", "機械学習": "画像処理・AI・Deep Learning",
            "Deep Learning": "画像処理・AI・Deep Learning",
            "システム": "システム・運用・自動化", "システム・運用・自動化": "システム・運用・自動化",
            "IoT": "IoT・センサー・ハード連携", "センサー": "IoT・センサー・ハード連携",
            "Raspberry Pi": "IoT・センサー・ハード連携"
        }

        # --- 対象列 ---
        filter_columns = ["年", "指導教員", "言語", "ジャンル"]

                

                # 常にAND条件で絞り込む
        st.session_state.logic_type = "AND条件"
        
        # --- 絞り込みUI作成 ---
        for col in filter_columns:
            st.markdown(f"### {col} で絞り込み")

            if f"multi_{col}" not in st.session_state:
                st.session_state[f"multi_{col}"] = ["すべて表示"]

            if col not in st.session_state.selected_filters:
                st.session_state.selected_filters[col] = st.session_state[f"multi_{col}"]

            if col == "言語":
                unique_values = df["言語"].dropna().unique()
                mapped_values = set()
                for val in unique_values:
                    langs = [s.strip() for s in str(val).replace("/", ",").replace("、", ",").split(",")]
                    for lang in langs:
                        for key, unified_name in language_mapping.items():
                            if key.lower() in lang.lower():
                                mapped_values.add(unified_name)
                options = ["すべて表示"] + sorted(mapped_values)

            elif col == "ジャンル":
                unique_values = df["ジャンル"].dropna().unique()
                mapped_values = set()
                for val in unique_values:
                    genres = [s.strip() for s in str(val).replace("/", ",").replace("、", ",").split(",")]
                    for genre in genres:
                        for key, unified_name in genre_mapping.items():
                            if key.lower() in genre.lower():
                                mapped_values.add(unified_name)
                options = ["すべて表示"] + sorted(mapped_values)

            else:
                options = ["すべて表示"] + sorted(df[col].dropna().unique())

            st.multiselect(
                f"{col}を選んでください（複数可）",
                options,
                key=f"multi_{col}"
            )

            selected_values = st.session_state[f"multi_{col}"]
            if "すべて表示" in selected_values and len(selected_values) > 1:
                selected_values = ["すべて表示"]
            st.session_state.selected_filters[col] = selected_values

            # --- 🔍 キーワード検索欄を追加 ---
        keyword = st.text_input("🔍 キーワード検索（タイトル・概要・教員・言語に含まれる語で絞り込み）")
        
        if "keyword_history" not in st.session_state:
            st.session_state.keyword_history = load_keyword_history()

        if keyword.strip():
           kw = keyword.strip()
           st.session_state.keyword_history.append(kw)  # 毎回履歴に追加
           save_keyword_history(st.session_state.keyword_history)
        if st.session_state.keyword_history:
            st.markdown("### よく検索されているキーワード")
            keyword_series = pd.Series(st.session_state.keyword_history)
            keyword_counts = keyword_series.value_counts().head(6)

            cols = st.columns([1,1,1,1,1,1])
            for i, (kw,count) in enumerate(keyword_counts.items()):
                col = cols[i%6]
                with col:
                    if st.button(f"{kw}({count}回)",key=f"kwbtn_{i}"):
                        st.session_state.selected_keyword = kw
                        st.session_state.keyword_input = kw
                        st.rerun()
                        
        if "selected_keyword" in st.session_state:
            keyword = st.session_state.selected_keyword
        # --- フィルタ処理 ---
        filtered_df = df.copy()
        filters = st.session_state.selected_filters
        logic_type = st.session_state.logic_type
        
        if any(filters.values()):
            conditions = []
            for col, values in filters.items():
                if not values or values == ["すべて表示"]:
                    continue
                if col == "言語":
                    cond = df["言語"].apply(
                        lambda x: any(language_mapping.get(k, k) in values for k in str(x).replace("/", ",").split(","))
                        if pd.notna(x) else False
                    )
                elif col == "ジャンル":
                    cond = df["ジャンル"].apply(
                        lambda x: any(genre_mapping.get(k, k) in values for k in str(x).replace("/", ",").split(","))
                        if pd.notna(x) else False
                    )
                else:
                    cond = df[col].isin(values)
                conditions.append(cond)
            
            if len(conditions) > 0:
                combined = conditions[0]
                for cond in conditions[1:]:
                    if logic_type == "AND条件":
                        combined = combined & cond
                    else:
                        combined = combined | cond
                filtered_df = df[combined]
            if keyword.strip():
                keyword_lower = keyword.lower()
                filtered_df = filtered_df[
                    filtered_df.apply(
                        lambda row: any(
                            keyword_lower in str(row[col]).lower()
                            for col in ["タイトル","概要","指導教員","言語"]
                        ),
                        axis=1
                    )]
        # --- 検索結果表示 ---
        st.write(f"検索結果：{len(filtered_df)} 件")
        st.data_editor(filtered_df.drop("ID", axis=1), hide_index=True, use_container_width=True)
        # --- グラフ ---
        chart_type = st.radio("グラフ表示", ["使用言語の分布", "ジャンルの分布"], horizontal=True)
        fig, ax = plt.subplots(figsize=(6, 3))

        if chart_type == "使用言語の分布" and "言語" in df.columns:
            st.subheader("使用言語の分布")
            categories = ["C", "C#", "C++", "Java", "JavaScript", "Python", "PHP", "html/css", "その他"]
            lang_count = {cat: 0 for cat in categories}

            language_mapping_chart = {
                "C": "C", "C言語": "C", "C++": "C++", "C#": "C#",
                "Python": "Python","python":"Python",  "Java": "Java","java":"Java", "JavaScript": "JavaScript",
                "JS": "JavaScript","javascript":"JavaScript","Javascript":"JavaScript", "PHP": "PHP","php":"PHP",
                "html/css": "html/css","HTML":"html/css","html":"html/css","HTML5":"html/css"
            }

            for langs in df["言語"].dropna():
                lang_list = [s.strip() for s in str(langs).replace("、", ",").split(",")]
                for lang in lang_list:
                    l = language_mapping_chart.get(lang, "その他")
                    if l in lang_count:
                        lang_count[l] += 1
                    else:
                        lang_count["その他"] += 1
            bars = ax.bar(lang_count.keys(),lang_count.values(),color="#4CAF50")


            #件数を実際に棒の上に表示
            for bar in bars:
                height = bar.get_height()
                ax.text(
                   bar.get_x() + bar.get_width()/2,
                   height,
                   f"{int(height)}",
                   ha="center",va="bottom",fontsize=8
                )

            
            ax.set_ylabel("使\n用\n件\n数",rotation=0,labelpad=15)
            ax.set_xlabel("言語")
            ax.set_xticklabels(lang_count.keys(), fontsize=6)
            ax.set_ylim(0, max(lang_count.values()) + 1)

        elif chart_type == "ジャンルの分布" and "ジャンル" in df.columns:
            st.subheader("ジャンルの分布")
            genre_count = df["ジャンル"].value_counts()
            bars = ax.bar(genre_count.index, genre_count.values, color="#42A5F5")

           #件数を実際に棒の上に表示
            for bar in bars:
                height = bar.get_height()
                ax.text(
                   bar.get_x() + bar.get_width()/2,
                   height,
                   f"{int(height)}",
                   ha="center",va="bottom",fontsize=8
                )

            ax.set_ylabel("件\n数",rotation=0,labelpad=15)
            ax.set_xlabel("ジャンル")
            ax.set_xticklabels(genre_count.index, rotation=30, ha="right", fontsize=6)
            ax.set_ylim(0, max(genre_count.values) + 1)

        st.pyplot(fig, use_container_width=True)
        # --- 使用言語 or ジャンル の切り替え ---
        


        
# --- 資料リンク画面 ---
elif selected == "資料":
    st.markdown(
    """
    <style>
    .center-title {
        text-align: center;
        font-size: 56px;
        font-weight: bold;
        
        margin-bottom: 20px;
    }
    </style>
    <div class="center-title">研究資料ページ</div>
    """,
    unsafe_allow_html=True
)
    st.write("各研究のPowerPointやWord資料にアクセスできます。")

    st.markdown(
        """
        <style>
        body {
            font-family: 'Segoe UI', 'Roboto', sans-serif;
            color: #333;
            background-color: #fafafa;
        }
        .table {
            border-collapse: collapse;
            width: 100%;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
        }
        .table-cell {
            display: flex;
            align-items: center;     /* 縦方向中央 */
            justify-content: center; /* 横方向中央 */
            height: 100%;
            height: 100%;               /* 縦幅もセルいっぱいに */
            padding: 20px;              /* 余白調整 */
            box-sizing: border-box; 
        }
        .table-header, .table th {
            font-weight: 600;
            background: linear-gradient(135deg, #e8f5e9, #c8e6c9);
            border-bottom: 2px solid #81c784;
            padding: 4px 8px;
            text-align: center;
            color: #2e7d32;
            display: flex;
            align-items: center;
            justify-content: center;
            letter-spacing: 0.3px;
            font-size: 14px;
            min-height: 44px;
        }
        .table td {
            text-align: center;     
            border-bottom: 1px solid #ddd;
            padding: 0;
            text-align: center;
            background-color: #fff;
            transition: background-color 0.3s ease;
            vertical-align: middle;
            min-height: 70px;
        }
        div.stDownloadButton > button {
            display: block;
            margin: 6px auto;
            width: 90%;
            padding: 12px 16px;
            font-size: 14px;
            font-weight: 600;
            color: white;
            background: linear-gradient(135deg, #66bb6a, #43a047);
            border: none;
            border-radius: 24px;
            cursor: pointer;
            transition: all 0.25s ease;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
        }
        div.stDownloadButton > button:hover {
            background: linear-gradient(135deg, #81c784, #66bb6a);
            transform: translateY(-3px);
            box-shadow: 0 6px 14px rgba(0, 0, 0, 0.15);
        }
        div.stDownloadButton > button:active {
            background: linear-gradient(135deg, #388e3c, #2e7d32);
            transform: translateY(0);
            box-shadow: 0 3px 8px rgba(0, 0, 0, 0.1);
        }
        </style>
        """,
        unsafe_allow_html=True
    )

    @st.cache_data
    def load_data():
        return pd.read_excel("卒業研究リンク.xlsx")

    df_links = load_data()
    df_links["Word_exists"] = df_links["報告書"].apply(lambda x: pd.notna(x) and os.path.exists(x))

    search_query = st.text_input("タイトルで検索")
    col_sort1, col_sort2 = st.columns([2, 1])
    with col_sort1:
        sort_col = st.selectbox("並び替え項目", ["年", "タイトル"])
    with col_sort2:
        sort_order = st.radio("順序", ["昇順", "降順"], horizontal=True, index=1)

    filtered_df = df_links.copy()
    if search_query:
        filtered_df = filtered_df[filtered_df["タイトル"].str.contains(search_query, case=False, na=False)]
    filtered_df = filtered_df.sort_values(by=sort_col, ascending=(sort_order == "昇順"))

    col1, col2, col3, col4, col5 = st.columns([1, 1.5, 1, 1, 1])
    with col1:
        st.markdown('<div class="table-header hide-mobile">年</div>', unsafe_allow_html=True)
    with col2:
        st.markdown('<div class="table-header hide-mobile">タイトル</div>', unsafe_allow_html=True)
    with col3:
        st.markdown('<div class="table-header hide-mobile">予稿</div>', unsafe_allow_html=True)
    with col4:
        st.markdown('<div class="table-header hide-mobile">パネル</div>', unsafe_allow_html=True)
    with col5:
        st.markdown('<div class="table-header hide-mobile">報告書</div>', unsafe_allow_html=True)

    counter = 0

    for idx, row in filtered_df.iterrows():
        tosi = row["年"]
        title = row["タイトル"]
        yokou = row["予稿"]
        panel = row["パネル"]
        houkoku = row["報告書"]
    
        # 各行をコンテナで囲む
        with st.container():
            col1, col2, col3, col4, col5 = st.columns([1, 1.5, 1, 1, 1])
            with col1:
                st.markdown(f'<div class="table-cell">{tosi}</div>', unsafe_allow_html=True)
            with col2:
                st.markdown(f'<div class="table-cell">{title}</div>', unsafe_allow_html=True)
            with col3:
                if pd.notna(yokou) and os.path.exists(yokou):
                    ext = os.path.splitext(yokou)[1].lower()
                    mime = "application/pdf" if ext == ".pdf" else "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                    with open(yokou, "rb") as f:
                        st.download_button("予稿", f, file_name=os.path.basename(yokou),
                                           mime=mime,
                                           key=f"yokou-{idx}")
                else:
                    st.markdown('<div class="table-cell">なし</div>', unsafe_allow_html=True)
            with col4:
                if pd.notna(panel) and os.path.exists(panel):
                    ext = os.path.splitext(yokou)[1].lower()
                    mime = "application/pdf" if ext == ".pdf" else "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                    with open(panel, "rb") as f:
                        st.download_button("パネル", f, file_name=os.path.basename(panel),
                                           mime=mime, key=f"panel-{idx}")
                else:
                    st.markdown('<div class="table-cell">なし</div>', unsafe_allow_html=True)
            with col5:
                if pd.notna(houkoku) and os.path.exists(houkoku):
                    ext = os.path.splitext(yokou)[1].lower()
                    mime = "application/pdf" if ext == ".pdf" else "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                    with open(houkoku, "rb") as f:
                        st.download_button("報告書", f, file_name=os.path.basename(houkoku),
                                           mime=mime,
                                           key=f"houkoku-{idx}")
                else:
                    st.markdown('<div class="table-cell">なし</div>', unsafe_allow_html=True)
elif selected == "使い方":
    st.markdown(
        """
        <style>
        .center-title{
            text-align: center;
            font-size: 56px;
            font-weight: bold;
            margin-bottom: 20px;
        }
        </style>
        <div class="center-title"> ウェブサイトの使い方ページ</div>
        """,
    unsafe_allow_html=True
)
    
    
    st.markdown("---")


    st.markdown('<p class="feature-heading"><strong>追加</strong></p>', unsafe_allow_html=True)
    st.markdown("このページでは新しい卒業研究データを追加できます。年、タイトル、指導教員、ジャンル、使用言語、概要の６個の項目を入力し「追加」ボタンをクリックして追加できます。<br>画面下側で研究データ一覧表を見ることができ、現在登録されているデータを確認することができます。",unsafe_allow_html=True)
    st.markdown('<p class="feature-heading"><strong>編集</strong></p>', unsafe_allow_html=True)
    st.markdown("このページでは登録済みの研究データを編集したり、完全に削除することができます。研究データを選択し、年、タイトル、指導教員、ジャンル、使用言語、概要の各項目を<br>自由に書き換えることができます。変更を保存する場合は「更新する」のボタンをクリックしてください。研究データを削除する場合は「削除する」のボタンをクリックしてください。<br>追加ページと同様、画面下側では現在登録されている研究データを確認することができます。",unsafe_allow_html=True)
    st.markdown('<p class="feature-heading"><strong>閲覧・検索</strong></p>', unsafe_allow_html=True)
    st.markdown("このページでは研究データの閲覧と検索ができます。2010～2024年までのデータが年、タイトル、指導教員、言語、ジャンル、概要の６個の項目に分けて一覧表で登録されています。",unsafe_allow_html=True)
    st.markdown("　　主な機能",unsafe_allow_html=True)
    st.markdown("　　・詳細な絞込み：各項目（年、指導教員、言語、ジャンル）ごとに複数の条件を選択して、見たいデータを絞り込めます。",unsafe_allow_html=True)
    st.markdown("　　・検索ヒント：検索に迷った際は、検索欄の下に表示される**「よく検索されているキーワード」**が役立ちます。",unsafe_allow_html=True)
    st.markdown("　　・データ可視化: 使用言語と研究ジャンルの分布を棒グラフで表示し、全体の傾向を直感的に把握できます。",unsafe_allow_html=True)
    st.markdown('<p class="feature-heading"><strong>資料</strong></p>', unsafe_allow_html=True)
    st.markdown("このページでは、過去の先輩方が作成した卒業研究の資料を閲覧できます。予稿・パネル・報告書の3種類があり、見たい資料のボタンをクリックすると、対応するPDFが開きます。",unsafe_allow_html=True)
    st.markdown("---")
    