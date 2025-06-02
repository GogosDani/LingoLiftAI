import React, { useState, useEffect } from 'react';
import { api } from '../axios/api';

interface Topic {
    id: number;
    name: string;
    description: string;
}

interface PopularTopic extends Topic {
    popularity: number;
}

interface Wordset {
    id: number;
    name: string;
    description: string;
    difficultyLevel: string;
    wordCount: number;
    topicName: string;
    createdAt: string;
}

interface CreateWordsetForm {
    topicId: string;
    difficultyLevel: string;
    wordCount: number;
}

type DifficultyLevel = "beginner" | "elementary" | "intermediate" | "upper intermediate" | "advanced" | "proficient";

export default function AiWordsets() {
    const [wordsets, setWordsets] = useState<Wordset[]>([]);
    const [topics, setTopics] = useState<Topic[]>([]);
    const [popularTopics, setPopularTopics] = useState<PopularTopic[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string>('');
    const [showCreateForm, setShowCreateForm] = useState<boolean>(false);
    const [isGenerating, setIsGenerating] = useState<boolean>(false);

    const [createForm, setCreateForm] = useState<CreateWordsetForm>({
        topicId: '',
        difficultyLevel: 'beginner',
        wordCount: 10
    });

    const difficultyLevels: DifficultyLevel[] = [
        "beginner", "elementary", "intermediate",
        "upper intermediate", "advanced", "proficient"
    ];

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async (): Promise<void> => {
        try {
            setIsLoading(true);
            setError('');

            const [wordsetsResponse, topicsResponse, popularTopicsResponse] = await Promise.all([
                api.get<Wordset[]>('/api/ai-wordset/my-wordsets'),
                api.get<Topic[]>('/api/ai-wordset/topics'),
                api.get<PopularTopic[]>('/api/ai-wordset/popular-topics?count=5')
            ]);
            setWordsets(wordsetsResponse.data);
            setTopics(topicsResponse.data);
            setPopularTopics(popularTopicsResponse.data);
        } catch (err: any) {
            console.error('Error loading data:', err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleCreateNew = (): void => {
        setShowCreateForm(true);
        setCreateForm({
            topicId: '',
            difficultyLevel: 'beginner',
            wordCount: 10
        });
    };

    const handleGenerateWordset = async (): Promise<void> => {
        if (!createForm.topicId) {
            setError('Please select a topic');
            return;
        }
        try {
            setIsGenerating(true);
            setError('');
            const response = await api.post('/api/ai-wordset/generate', {
                topicId: parseInt(createForm.topicId),
                difficultyLevel: createForm.difficultyLevel,
                wordCount: createForm.wordCount
            });
            setShowCreateForm(false);
            await loadData();

        } catch (err: any) {
            console.error('Error generating wordset:', err);
        } finally {
            setIsGenerating(false);
        }
    };

    const handleViewWordset = (id: number): void => {
        console.log('View wordset:', id);
    };

    const handleDelete = async (e: React.MouseEvent<HTMLButtonElement>, id: number): Promise<void> => {
        e.stopPropagation();
        if (!confirm('Are you sure you want to delete this wordset?')) {
            return;
        }

        try {
            await api.delete(`/api/ai-wordset/${id}`);
            setWordsets(wordsets.filter(ws => ws.id !== id));
        } catch (err: any) {
            console.error('Error deleting wordset:', err);
        }
    };

    const getDifficultyColor = (level: string): string => {
        const colors: Record<string, string> = {
            'beginner': 'bg-green-100 text-green-800',
            'elementary': 'bg-blue-100 text-blue-800',
            'intermediate': 'bg-yellow-100 text-yellow-800',
            'upper intermediate': 'bg-orange-100 text-orange-800',
            'advanced': 'bg-red-100 text-red-800',
            'proficient': 'bg-purple-100 text-purple-800'
        };
        return colors[level] || 'bg-gray-100 text-gray-800';
    };

    const formatDate = (dateString: string): string => {
        return new Date(dateString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    };

    const handleFormChange = (field: keyof CreateWordsetForm, value: string | number): void => {
        setCreateForm(prev => ({
            ...prev,
            [field]: value
        }));
    };

    if (showCreateForm) {
        return (
            <div className="min-h-screen bg-gray-50">
                <div className="pt-16 px-4 md:px-12 max-w-4xl mx-auto">
                    <div className="text-center my-8">
                        <h1 className="text-3xl font-bold text-gray-800">Generate AI Wordset</h1>
                        <p className="text-gray-600 mt-2">Create a personalized wordset using AI</p>
                    </div>
                    {error && (<div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4"> {error} </div>)}
                    <div className="bg-white rounded-xl shadow-md p-8">
                        <div className="space-y-6">
                            <div>
                                <div className="block text-sm font-medium text-gray-700 mb-2"> Popular Topics </div>
                                <div className="grid grid-cols-1 md:grid-cols-3 gap-3 mb-4">
                                    {popularTopics.map(topic => (
                                        <button key={topic.id} onClick={() => handleFormChange('topicId', topic.id.toString())} className={`p-3 text-left rounded-lg border transition-colors ${createForm.topicId === topic.id.toString() ? 'border-blue-500 bg-blue-50' : 'border-gray-200 hover:border-gray-300'}`} >
                                            <div className="font-medium text-gray-900">{topic.name}</div>
                                            <div className="text-sm text-gray-500">{topic.description}</div>
                                        </button>
                                    ))}
                                </div>
                            </div>
                            <div>
                                <div className="block text-sm font-medium text-gray-700 mb-2">  Or select from all topics </div>
                                <select value={createForm.topicId} onChange={(e) => handleFormChange('topicId', e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" >
                                    <option value="">Choose a topic...</option>
                                    {topics.map(topic => (
                                        <option key={topic.id} value={topic.id}>
                                            {topic.name} - {topic.description}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div>
                                <div className="block text-sm font-medium text-gray-700 mb-2"> Difficulty Level </div>
                                <select value={createForm.difficultyLevel} onChange={(e) => handleFormChange('difficultyLevel', e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" >
                                    {difficultyLevels.map(level => (
                                        <option key={level} value={level}>
                                            {level.charAt(0).toUpperCase() + level.slice(1)}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div>
                                <div className="block text-sm font-medium text-gray-700 mb-2"> Number of Words (1-50) </div>
                                <input type="number" min="1" max="50" value={createForm.wordCount} onChange={(e) => handleFormChange('wordCount', parseInt(e.target.value) || 1)} className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" />
                            </div>
                        </div>
                        <div className="flex gap-4 mt-8">
                            <button onClick={() => setShowCreateForm(false)} className="flex-1 bg-gray-500 hover:bg-gray-600 text-white px-6 py-3 rounded-lg transition-colors" > Cancel </button>
                            <button onClick={handleGenerateWordset} disabled={isGenerating || !createForm.topicId} className="flex-1 bg-blue-500 hover:bg-blue-600 disabled:bg-blue-300 text-white px-6 py-3 rounded-lg transition-colors flex items-center justify-center" >
                                {isGenerating ? (
                                    <>
                                        <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                                        Generating...
                                    </>
                                ) : (
                                    'Generate with AI'
                                )}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50">
            <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="text-center my-8">
                    <h1 className="text-3xl font-bold text-gray-800">My AI Wordsets</h1>
                    <p className="text-gray-600 mt-2">Wordsets generated by AI</p>
                </div>
                {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4"> {error}  </div>
                )}
                <div className="flex justify-end mb-6">
                    <button onClick={handleCreateNew} className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-md flex items-center" > Generate New AI Wordset </button>
                </div>
                {isLoading ? (
                    <div className="text-center py-12">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500 mx-auto mb-4"></div>
                        <p className="text-gray-600">Loading AI wordsets...</p>
                    </div>
                ) : wordsets.length === 0 ? (
                    <div className="bg-white rounded-xl shadow-md p-10 mb-8 text-center">
                        <div className="text-6xl mb-4">🤖</div>
                        <p className="text-gray-600 text-lg mb-2">You don't have any AI wordsets yet.</p>
                        <p className="text-gray-500 mb-6">Generate your first wordset!</p>
                        <button onClick={handleCreateNew} className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-md" > Generate AI Wordset </button>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
                        {wordsets.map((wordset) => (
                            <div key={wordset.id} className="bg-white rounded-xl shadow-md hover:shadow-lg transition-shadow p-6 cursor-pointer relative" onClick={() => handleViewWordset(wordset.id)} >
                                <h2 className="text-xl font-semibold text-gray-800 mb-2 pr-8"> {wordset.name}  </h2>
                                <div className="flex items-center mb-3">
                                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${getDifficultyColor(wordset.difficultyLevel)}`}> {wordset.difficultyLevel} </span>
                                </div>
                                <div className="text-sm text-gray-600 mb-3"> Topic:
                                    <span className="font-medium">{wordset.topicName}</span>
                                </div>
                                <div className="flex items-center justify-between text-sm text-gray-500">
                                    <span>{wordset.wordCount} words</span>
                                    <span>{formatDate(wordset.createdAt)}</span>
                                </div>
                                <div className="absolute top-2 right-2 flex items-center">
                                    <button onClick={(e) => handleDelete(e, wordset.id)} className="text-gray-400 hover:text-red-500 transition-colors bg-transparent p-1 rounded-full hover:bg-red-50" > ✕ </button>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};