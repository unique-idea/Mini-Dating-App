import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import PhoneLayout from '../components/PhoneLayout'
import axiosClient from '../services/axiosClientService'
import { onMatchCreated, onMatchUpdated, offMatchCreated, offMatchUpdated } from '../services/signalRService'
import '../index.css'

function MatchesPage() {
    const MATCH_STATUS = {
        MATCHED: 'Matched',
        PENDING: 'Pending',
        NOSLOTFOUND: 'NoSlotFound',
        SCHEDULED: 'Scheduled'
    };

    const navigate = useNavigate()
    const [matches, setMatches] = useState([])
    const [hasAvailability, setHasAvailability] = useState(false)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')
    const [selectedMatch, setSelectedMatch] = useState(null)
    const [scheduling, setScheduling] = useState(null)
    const [scheduleMsg, setScheduleMsg] = useState('')
    const currentUser = JSON.parse(localStorage.getItem('currentUser'))

    const getScheduleMsg = (status) => {
        switch (status) {
            case MATCH_STATUS.PENDING:
                return { msg: 'Your request has been sent, waiting for partner to confirm 🤞', type: 'success' }
            case MATCH_STATUS.SCHEDULED:
                return { msg: 'Match are now scheduled 🎉', type: 'success' }
            case MATCH_STATUS.NO_SLOT_FOUND:
                return { msg: 'No common slot found. Please update your availability and try again 📅', type: 'warning' }
            default:
                return { msg: 'Match status updated!', type: 'info' }
        }
    }

    const MSG_STYLES = {
        success: { background: '#e8f5e9', border: '#a5d6a7', color: '#28a745' },
        warning: { background: '#fff3e0', border: '#ffcc80', color: '#ff9800' },
        error: { background: '#fce4ec', border: '#f48fb1', color: '#e0245e' },
        info: { background: '#e3f2fd', border: '#90caf9', color: '#1976d2' }
    }

    const sortMatches = (matches) => {
        return [...matches].sort((a, b) => {
            if (a.scheduledDate && b.scheduledDate) {
                return new Date(a.scheduledDate.date) - new Date(b.scheduledDate.date)
            }

            if (a.scheduledDate && !b.scheduledDate) return -1

            if (!a.scheduledDate && b.scheduledDate) return 1
            return 0
        })
    }

    const fetchMatches = async () => {
        try {
            const res = await axiosClient.get('/Match')
            setMatches(sortMatches(res.data.matches))
            setHasAvailability(res.data.hasAvailability)
        } catch (err) {
            setError('Failed to load matches. Try again later! @@')
        } finally {
            setLoading(false)
        }
    }
    {/* Handel Schedule Message */ }
    useEffect(() => {
        if (!scheduleMsg) return
        const timer = setTimeout(() => setScheduleMsg(null), 2500) 
        return () => clearTimeout(timer)
    }, [scheduleMsg])

    {/* Handel Refresh Matches */ }
    useEffect(() => {
        fetchMatches()
    }, [])

    {/* Handel SignalR */ }

    useEffect(() => {
        onMatchCreated(() => {
            console.log('🎉 MatchCreated')
            fetchMatches()
        })

        onMatchUpdated((data) => {
            console.log('🔄 MatchUpdated:', data)
            setMatches(prev => sortMatches(prev.map(m =>
                m.matchId === data.matchId
                    ? {
                        ...m,
                        status: data.status,
                        scheduledDate: data.scheduledDate,
                        userMatchedConfirmed: data.userMatchedConfirmed
                    }
                    : m
            )))
        })

        return () => {
            offMatchCreated()
            offMatchUpdated()
        }
    }, [])


    const handleSchedule = async (matchId) => {
        setScheduling(matchId)
        setScheduleMsg('')
        try {
            const res = await axiosClient.post(`/Match/schedule/${matchId}`)
            const updatedMatch = res.data

            setMatches(prev => sortMatches(prev.map(m =>
                m.matchId === updatedMatch.matchId
                    ? { ...m, status: updatedMatch.status, scheduledDate: updatedMatch.scheduledDate }
                    : m
            )))

            setScheduleMsg(getScheduleMsg(updatedMatch.status))

        } catch (err) {
            setScheduleMsg({
                msg: err.response?.data?.message || 'Failed to schedule. Try again later! @@',
                type: 'error'
            })
        } finally {
            setScheduling(null)
        }
    }

    const getStatusStyle = (status) => {
        switch (status) {
            case 'Matched':
                return { color: '#e0245e', background: '#fce4ec', border: '1px solid #ffccd5' };
            case 'Pending':
                return { color: '#ff9800', background: '#fff3e0', border: '1px solid #cce5ff' };
            case 'Scheduled':
                return { color: '#28a745', background: '#e8f5e9', border: '1px solid #d4edda' };
            case 'NoSlotFound':
                return { color: '#9c27b0', background: '#f3e5f5', border: '1px solid #ffeeba' };
            default:
                return { color: '#aaa', background: '#f5f5f5', border: '1px solid #e9ecef' };
        }
    }
    const getStatusHint = (status, userMatchedConfirmed) => {
        switch (status) {
            case 'Matched': return { label: "Match found let's request a date! 💕", color: '#e0245e', bg: '#fce4ec' }
            case 'Pending': return { label: userMatchedConfirmed ? "It's your turn !" : 'Waiting for partner... ⏳', color: '#ff9800', bg: '#fff3e0' }
            case 'NoSlotFound': return { label: 'No slot found fix schedule! 🔄', color: '#9c27b0', bg: '#f3e5f5' }
            case 'Scheduled': return { label: 'Tap to view your date! 🎉', color: '#28a745', bg: '#e8f5e9' }
            default: return { label: status, color: '#aaa', bg: '#f5f5f5' }
        }
    }

    return (
        <PhoneLayout onAvailabilitySet={fetchMatches}>
            <div style={{ padding: '20px' }}>
                <h6 style={{ fontWeight: 700, marginBottom: '4px' }}>Your Matches</h6>
                <p style={{ fontSize: '12px', color: '#aaa', marginBottom: '16px' }}>
                    {matches.length} match{matches.length !== 1 ? 'es' : ''} found
                </p>

                {/* No availability */}
                {!hasAvailability && (
                    <div style={{
                        background: '#fff3e0',
                        border: '1px solid #ffcc80',
                        borderRadius: '12px',
                        padding: '10px 14px',
                        fontSize: '12px',
                        color: '#e65100',
                        marginBottom: '16px',
                        display: 'flex',
                        alignItems: 'center',
                        gap: '8px'
                    }}>
                        Well look like you haven't set your availability yet. Set it to schedule dates!
                    </div>
                )}

                {/* Schedule Message */}
                {scheduleMsg && (
                    <div style={{
                        background: MSG_STYLES[scheduleMsg.type].background,
                        border: `1px solid ${MSG_STYLES[scheduleMsg.type].border}`,
                        borderRadius: '12px',
                        padding: '10px 14px',
                        fontSize: '12px',
                        color: MSG_STYLES[scheduleMsg.type].color,
                        marginBottom: '16px',
                        textAlign: 'center'
                    }}>
                        {scheduleMsg.msg}
                    </div>
                )}

                {/* Loading */}
                {loading && (
                    <div style={{ textAlign: 'center', padding: '40px', color: '#aaa' }}>
                        <div className="spinner-border text-danger spinner-border-sm" />
                        <p style={{ marginTop: '8px', fontSize: '13px' }}>Loading matches...</p>
                    </div>
                )}

                {/* Error */}

                {error && (
                    <div style={{ textAlign: 'center', padding: '40px', color: '#e0245e', fontSize: '14px' }}>
                        Opps we got some problems ⚠️ {error}
                    </div>
                )}

                {/* Empty */}
                {!loading && !error && matches.length === 0 && (
                    <div style={{ textAlign: 'center', padding: '40px', color: '#aaa' }}>
                        <div style={{ fontSize: '48px' }}>💔</div>
                        <p style={{ fontSize: '14px', marginTop: '8px' }}>No matches yet!</p>
                        <p style={{ fontSize: '10px', marginTop: '3px' }}>Since every like can lead to a real date, matches might take time, so keep liking more interested  people</p>
                    </div>
                )}

                {/* Match list */}
                {matches.map(match => {
                    const statusStyle = getStatusStyle(match.status)
                    const genderIcon = match.user.gender === 'Female' ? '👩' : match.user.gender === 'Male' ? '👨' : '🧑'
                    const isSelected = selectedMatch?.matchId === match.matchId

                    return (
                        <div key={match.matchId} style={{ marginBottom: '10px' }}>
                            <div style={{
                                display: 'flex',
                                alignItems: 'center',
                                gap: '10px',
                            }}>
                                {/* Match card */}
                                <div style={{
                                    flex: 1,
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: '12px',
                                    background: 'white',
                                    borderRadius: '16px',
                                    padding: '12px',
                                    boxShadow: '0 2px 12px rgba(0,0,0,0.08)',
                                    border: '1px solid #f5f5f5',
                                    cursor: 'pointer',
                                    minHeight: '65px'
                                }}
                                    onClick={() => setSelectedMatch(isSelected ? null : match)}
                                    className={match.status === 'Scheduled' ? 'scheduled-glow' : ''}
                                >
                                    {/* Avatar */}
                                    <div style={{
                                        width: '42px', height: '42px',
                                        borderRadius: '14px',
                                        background: 'linear-gradient(135deg, #fce4ec, #f8bbd0)',
                                        display: 'flex', alignItems: 'center',
                                        justifyContent: 'center',
                                        fontSize: '22px', flexShrink: 0
                                    }}>
                                        {genderIcon}
                                    </div>

                                    {/* Info */}
                                    <div style={{ flex: 1, minWidth: 0 }}>
                                        <div style={{ fontWeight: 700, fontSize: '13px' }}>
                                            {match.user.name}, {match.user.age}
                                        </div>
                                        <div style={{
                                            fontSize: '8px',
                                            marginTop: '2px',
                                            color: getStatusHint(match.status).color,
                                            fontWeight: 500
                                        }}>
                                            {getStatusHint(match.status, match.userMatchedConfirmed).label}
                                        </div>
                                    </div>

                                    {/* Status badge */}
                                    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: '3px', flexShrink: 0 }}>
                                        <div style={{
                                            fontSize: '10px', fontWeight: 600,
                                            padding: '3px 8px', borderRadius: '20px',
                                            color: getStatusStyle(match.status).color,
                                            background: getStatusStyle(match.status).bg
                                        }}>
                                            {match.status}

                                        </div>
                                        <span style={{
                                            fontSize: '9px',
                                            color: getStatusStyle(match.status).color,
                                            opacity: 0.9,
                                            fontWeight: 500,
                                            width: '100%', textAlign: 'center'
                                        }}>
                                            view
                                        </span>
                                    </div>

                                    {/* Green dot for scheduled */}
                                    {match.status === 'Scheduled' && (
                                        <div style={{
                                            width: '8px', height: '8px',
                                            borderRadius: '50%', background: '#28a745',
                                            flexShrink: 0, boxShadow: '0 0 6px rgba(40,167,69,0.6)'
                                        }} />
                                    )}
                                </div>

                                {/* Schedule button */}
                                {match.status !== 'Scheduled' && (
                                    <button
                                        disabled={!hasAvailability || scheduling === match.matchId}
                                        onClick={() => handleSchedule(match.matchId)}
                                        style={{
                                            width: '70px',
                                            height: '65px',
                                            borderRadius: '12px',
                                            border: 'none',
                                            background: hasAvailability
                                                ? 'linear-gradient(135deg, #e0245e, #ff6b6b)'
                                                : '#eee',
                                            color: hasAvailability ? 'white' : '#aaa',
                                            fontWeight: 600,
                                            fontSize: '10px',
                                            cursor: hasAvailability ? 'pointer' : 'not-allowed',
                                            boxShadow: hasAvailability ? '0 4px 12px rgba(224,36,94,0.3)' : 'none',
                                            flexShrink: 0,
                                            lineHeight: 1.3
                                        }}
                                    >
                                        {scheduling === match.matchId
                                            ? <span className="spinner-border spinner-border-sm" />
                                            : !hasAvailability
                                                ? 'Set avail. first'
                                                : "💕 Let's Date"
                                        }
                                    </button>
                                )}
                            </div>

                            {/* Match detail overlay */}
                            {isSelected && (
                                <>
                                    {/* Backdrop */}
                                    <div
                                        onClick={() => setSelectedMatch(null)}
                                        style={{
                                            position: 'absolute',
                                            inset: 0,
                                            background: 'rgba(0,0,0,0.85)',
                                            zIndex: 200,
                                            borderRadius: '24px'
                                        }}
                                    />

                                    {/* Detail panel */}
                                    <div style={{
                                        position: 'absolute',
                                        bottom: 0, left: 0, right: 0,
                                        zIndex: 201,
                                        backgroundColor: 'white',
                                        borderRadius: '24px 24px 0 0',
                                        padding: '24px',
                                        boxShadow: '0 -4px 24px rgba(0,0,0,0.15)',
                                        display: 'flex',
                                        flexDirection: 'column',
                                        gap: '12px'
                                    }}>
                                        {/* Handle */}
                                        <div style={{
                                            width: '40px', height: '4px',
                                            background: '#eee', borderRadius: '2px',
                                            alignSelf: 'center', marginBottom: '4px'
                                        }} />

                                        {/* Close */}
                                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginLeft: 'auto' }}>
                                            <button onClick={() => setSelectedMatch(null)} style={{
                                                border: 'none', background: 'none',
                                                fontSize: '18px', cursor: 'pointer', color: 'black'
                                            }}>Tap to close</button>
                                        </div>

                                        {/* Match Status Details */}
                                        {match.status === 'Matched' && (
                                            <div style={{
                                                background: 'linear-gradient(135deg, #fce4ec, #fce4ec22)',
                                                borderRadius: '12px', padding: '14px',
                                                fontSize: '13px', textAlign: 'center',
                                                border: '1px solid rgba(224,36,94,0.15)'
                                            }}>
                                                <div style={{ fontSize: '20px', marginBottom: '6px' }}>🎉</div>
                                                <div style={{ color: '#333', fontWeight: 700, fontSize: '14px' }}>
                                                    It's a Match!
                                                </div>
                                                <div style={{ color: 'green', fontSize: '12px', marginTop: '6px', lineHeight: 1.6 }}>
                                                    You and <strong>{match.user.name}</strong> liked each other!<br />
                                                    Set your availability then tap <strong>"Let's Date"</strong> to find a common free slot.
                                                </div>
                                                <div style={{ color: '#e0245e', fontWeight: 600, fontSize: '12px', marginTop: '8px' }}>
                                                    Don't wait too long! 😉
                                                </div>
                                            </div>
                                        )}

                                        {match.status === 'Pending' && (
                                            <div style={{
                                                background: 'linear-gradient(135deg, #fff3e0, #fff8e122)',
                                                borderRadius: '12px', padding: '14px',
                                                fontSize: '13px', textAlign: 'center',
                                                border: '1px solid rgba(255,152,0,0.2)'
                                            }}>
                                                <div style={{ fontSize: '28px', marginBottom: '6px' }}>⏳</div>
                                                <div style={{ color: '#333', fontWeight: 700, fontSize: '14px' }}>
                                                    {
                                                        match.userMatchedConfirmed
                                                            ? "It's your turn!"
                                                            : `Waiting for ${match.user.name}...`
                                                    }
                                                </div>
                                                <div style={{ color: 'green', fontSize: '12px', marginTop: '6px', lineHeight: 1.6 }}>
                                                    {match.userMatchedConfirmed
                                                        ? <><strong>{match.user.name}</strong> has already requested a date!<br />Set your availability then tap <strong>"Let's Date"</strong> to confirm.</>
                                                        : <>You've already requested a date!<br />Hang tight while <strong>{match.user.name}</strong> sets their availability and confirms.</>

                                                    }
                                                </div>
                                                <div style={{ color: '#ff9800', fontWeight: 600, fontSize: '12px', marginTop: '8px' }}>
                                                    {match.userMatchedConfirmed
                                                        ? 'Don\'t keep them waiting! 😊'
                                                        : 'We\'ll find the best slot once they respond! 🤞'

                                                    }
                                                </div>
                                            </div>
                                        )}

                                        {match.status === 'NoSlotFound' && (
                                            <div style={{
                                                background: 'linear-gradient(135deg, #f3e5f5, #ede7f622)',
                                                borderRadius: '12px', padding: '14px',
                                                fontSize: '13px', textAlign: 'center',
                                                border: '1px solid rgba(156,39,176,0.2)'
                                            }}>
                                                <div style={{ fontSize: '28px', marginBottom: '6px' }}>😕</div>
                                                <div style={{ color: '#333', fontWeight: 700, fontSize: '14px' }}>
                                                    No Common Slot Found
                                                </div>
                                                <div style={{ color: 'green', fontSize: '12px', marginTop: '6px', lineHeight: 1.6 }}>
                                                    You and <strong>{match.user.name}</strong> don't have overlapping free time yet.<br />
                                                    Update your availability with more slots, then tap <strong>"Let's Date"</strong> to try again!
                                                </div>
                                                <div style={{ color: '#9c27b0', fontWeight: 600, fontSize: '12px', marginTop: '8px' }}>
                                                    More slots = better chances! 📅
                                                </div>
                                            </div>
                                        )}

                                        {match.status === 'Scheduled' && match.scheduledDate && (
                                            <div style={{
                                                background: 'linear-gradient(135deg, #e8f5e9, #f1f8e922)',
                                                borderRadius: '12px', padding: '14px',
                                                fontSize: '13px', textAlign: 'center',
                                                border: '1px solid rgba(40,167,69,0.2)'
                                            }}>
                                                <div style={{ fontSize: '28px', marginBottom: '6px' }}>🥂</div>
                                                <div style={{ color: '#333', fontWeight: 700, fontSize: '14px' }}>
                                                    Date Confirmed!
                                                </div>
                                                <div style={{ color: 'green', fontSize: '12px', marginTop: '6px', lineHeight: 1.6 }}>
                                                    You have a date with <strong>{match.user.name}</strong> at:
                                                </div>
                                                <div style={{
                                                    margin: '10px 0',
                                                    padding: '8px 12px',
                                                    background: 'white',
                                                    borderRadius: '10px',
                                                    color: '#28a745',
                                                    fontWeight: 700,
                                                    fontSize: '13px',
                                                    boxShadow: '0 2px 8px rgba(40,167,69,0.15)'
                                                }}>
                                                    📅 {new Date(match.scheduledDate.date).toLocaleDateString('en-GB', {
                                                        weekday: 'long', day: '2-digit', month: 'short'
                                                    })}
                                                    <br />
                                                    🕐 {match.scheduledDate.startTime?.slice(0, 5)} – {match.scheduledDate.endTime?.slice(0, 5)}
                                                </div>
                                                <div style={{ color: '#e0245e', fontWeight: 600, fontSize: '12px' }}>
                                                    Don't miss it! 😉
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                </>
                            )}
                        </div>
                    )
                })}
            </div>
        </PhoneLayout>
    )
}

export default MatchesPage