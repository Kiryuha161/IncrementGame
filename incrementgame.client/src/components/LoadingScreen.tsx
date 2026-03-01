import styles from '../App.module.css';

export function LoadingScreen() {
    return (
        <div className={styles.container}>
            <div style={{ textAlign: 'center', padding: '50px' }}>
                <div className={styles.loader}></div>
                <p style={{ marginTop: '20px', color: '#666' }}>Загрузка игры...</p>
            </div>
        </div>
    );
}