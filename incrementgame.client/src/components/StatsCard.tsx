import styles from '../App.module.css';

interface StatsCardProps {
    label: string;
    value: string | number;
    icon: string;
}

export function StatsCard({ label, value, icon }: StatsCardProps) {
    return (
        <div className={styles.statCard}>
            <div className={styles.statLabel}>{label}</div>
            <div className={styles.statValue}>{icon} {value}</div>
        </div>
    );
}